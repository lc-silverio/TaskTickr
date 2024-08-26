using Newtonsoft.Json;
using System;
using System.Net.Http.Headers;
using System.Text;
using TaskTickr.Domain.DTO;
using TaskTickr.Domain.Interfaces;
using TaskTickr.Shared.Enums;

namespace TaskTickr.Domain.Services
{
    /// <summary>
    /// Defines methods related with the communication with target Jira instance
    /// </summary>
    /// <seealso cref="TaskTickr.Domain.Interfaces.IJiraService" />
    public class JiraService : IJiraService
    {
        #region Properties
        /// <summary>
        /// The support logger service
        /// </summary>
        private readonly ISupportLoggerService _supportLoggerService;

        /// <summary>
        /// The settings service
        /// </summary>
        private readonly ISettingsService _settingsService;

        /// <summary>
        /// The HTTP client
        /// </summary>
        private readonly HttpClient _httpClient;

        /// <summary>
        /// The settings
        /// </summary>
        private readonly APISettings _settings;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="JiraService" /> class.
        /// </summary>
        /// <param name="supportLoggerService">The support logger service.</param>
        /// <param name="settingsService">The settings service.</param>
        public JiraService(ISupportLoggerService supportLoggerService, ISettingsService settingsService)
        {
            _supportLoggerService = supportLoggerService;
            _settingsService = settingsService;

            _settings = _settingsService.GetSettings();
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(_settings.URL)
            };
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_settings.Email}:{_settings.APIKey}")));
        }
        #endregion

        /// <summary>
        /// Gets the user tasks.
        /// </summary>
        /// <param name="targetEndpoint"></param>
        /// <param name="filterQuery"></param>
        /// <returns>
        /// Returns the users tasks
        /// </returns>
        /// <exception cref="System.Exception">Failed to get tasks for user {_settings.UserName} with error {response.StatusCode} and message {response.ReasonPhrase}</exception>
        public async Task<IEnumerable<JiraTask>> GetUserTasks(string targetEndpoint, string filterQuery)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{targetEndpoint + filterQuery}");

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Failed to get tasks for user {_settings.Email}. Error {response.ReasonPhrase}");
                }

                var content = await response.Content.ReadAsStringAsync();
                var jiraResponse = JsonConvert.DeserializeObject<JiraResponse>(content) ?? throw new Exception($"Failed to deserialize tasks for user {_settings.Email}");
                _supportLoggerService.AddLog($"Loaded tasks for user {_settings.Email}", LogLevel.Information);
                return jiraResponse.Issues;
            }
            catch (Exception ex)
            {
                _supportLoggerService.AddLog(ex.Message, LogLevel.Error);
                throw;
            }
        }

        /// <summary>
        /// Gets the task names from list.
        /// </summary>
        /// <param name="tasks">The tasks.</param>
        /// <returns>Returns the task names</returns>
        public List<string>GetTaskNamesFromList(IEnumerable<JiraTask> tasks)
        {
            return tasks
                .Select(x => x.Fields.Summary)
                .OrderBy(x => x)
                .ToList();
        }

        /// <summary>
        /// Gets the task names.
        /// </summary>
        /// <param name="targetEndpoint">The target endpoint.</param>
        /// <param name="filterQuery">The filter query.</param>
        /// <returns>Returns a list of task names</returns>
        public async Task<List<string>> GetTaskNames(string targetEndpoint, string filterQuery)
        {
            var result = await GetUserTasks(targetEndpoint, filterQuery);

            return result
                .Select(x => x.Fields.Summary)
                .OrderBy(x => x)
                .ToList();
        }

        /// <summary>
        /// Logs the task time.
        /// </summary>
        /// <param name="taskId">The task identifier.</param>
        /// <param name="taskName">The task identifier.</param>
        /// <param name="timeElapsed">The time elapsed.</param>
        /// <param name="startDate"></param>
        /// <param name="targetEndpoint"></param>
        /// <exception cref="System.Exception">Failed log time for task {taskName}. Error {response.ReasonPhrase}</exception>
        public async Task LogTaskTime(int taskId, string taskName, TimeSpan timeElapsed, DateTime startDate, string targetEndpoint)
        {
            try
            {
                var worklog = $@"
                    {{
                        ""timeSpentSeconds"": {(int)timeElapsed.TotalSeconds},
                        ""started"": ""{startDate.ToString("yyyy-MM-ddThh:mm:ss") + ".000+0000"}""
                    }}";

                var content = new StringContent(worklog, null, "application/json");
                var response = await _httpClient.PostAsync(targetEndpoint.Replace("TASK_ID", taskId.ToString()), content);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Failed log time for task {taskName}. Error {response.ReasonPhrase}");
                }

                _supportLoggerService.AddLog($"Logged {timeElapsed} into task {taskName}", LogLevel.Information);
            }
            catch (Exception ex)
            {
                _supportLoggerService.AddLog(ex.Message, LogLevel.Error);
                throw;
            }
        }
    }
}
