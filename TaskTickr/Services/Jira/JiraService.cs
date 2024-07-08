using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using TaskTickr.Services.Settings;
using TaskTickr.Services.SupportLogger;
using TaskTickr.Enums;
using System.Threading.Tasks;

namespace TaskTickr.Services.Jira
{
    /// <summary>
    /// Defines methods related with the communication with target Jira instance
    /// </summary>
    /// <seealso cref="IJiraService" />
    public class JiraService : IJiraService
    {
        #region Properties
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ISupportLoggerService _logger;

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

        /// <summary>
        /// The encoded user data
        /// </summary>
        private readonly byte[] _encodedUserData;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="JiraService"/> class.
        /// </summary>
        public JiraService(SupportLoggerService supportLoggerService, SettingsService settingsService)
        {
            _logger = supportLoggerService;

            // Load api settings
            _settingsService = settingsService;
            _settings = _settingsService.GetSettings();

            // Set target and credential settings
            _encodedUserData = Encoding.ASCII.GetBytes($"{_settings.UserName}:{_settings.Password}");
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(_settings.TargetInstanceURL)
            };
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(_encodedUserData));
        }
        #endregion

        /// <summary>
        /// Gets the user tasks.
        /// </summary>
        /// <returns>Returns the users tasks</returns>
        /// <exception cref="System.Exception">Failed to get tasks for user {_settings.UserName} with error {response.StatusCode} and message {response.ReasonPhrase}</exception>
        public async Task<IEnumerable<JiraTask>> GetUserTasks()
        {
            var url = _settings.TargetInstanceURL + Properties.Settings.Default.Jira_GetTasks;
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                _logger.AddLog($"Failed to get tasks for user {_settings.UserName} with error {response.StatusCode} and message {response.ReasonPhrase}", Enums.LogLevel.Error);
                throw new Exception($"Failed to get tasks for user {_settings.UserName} with error {response.StatusCode} and message {response.ReasonPhrase}");
            }

            var content = await response.Content.ReadAsStringAsync();

            _logger.AddLog($"Loaded tasks for user {_settings.UserName}", LogLevel.Information);
            return JsonConvert.DeserializeObject<JiraResponse>(content).Issues;
        }

        /// <summary>
        /// Logs the task time.
        /// </summary>
        /// <param name="taskId">The task identifier.</param>
        /// <param name="timeElapsed">The time elapsed.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public async Task LogTaskTime(string taskId, TimeSpan timeElapsed)
        {
            var time = timeElapsed.ToString();
            var content = new StringContent(JsonConvert.SerializeObject(new { time }), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_settings.TargetInstanceURL}/api/2/issue/{taskId}/worklog", content);

            if (!response.IsSuccessStatusCode) {
                _logger.AddLog($"Failed log time for task {taskId} with error code {response.StatusCode} and message {response.ReasonPhrase}", LogLevel.Error);
                throw new Exception($"Failed log time for task {taskId} with error code {response.StatusCode} and message {response.ReasonPhrase}");
            }

            _logger.AddLog($"Logged {timeElapsed} into task {taskId}", LogLevel.Information);
        }
    }
}
