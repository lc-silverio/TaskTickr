using TaskTickr.Domain.DTO;
using TaskTickr.Domain.Interfaces;
using TaskTickr.Shared.Enums;

namespace TaskTickr.Domain.Services
{
    /// <summary>
    /// Defines methods responsive for handling the processing of settings
    /// </summary>
    /// <seealso cref="TaskTickr.Services.Settings.ISettingsService" />
    public class SettingsService : ISettingsService
    {
        #region Properties

        /// <summary>
        /// The support logger service
        /// </summary>
        private readonly ISupportLoggerService _supportLoggerService;

        /// <summary>
        /// The ini settings file path
        /// </summary>
        private readonly string _filePath = $"{AppDomain.CurrentDomain.BaseDirectory}TaskTickrSettings.ini";

        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsService" /> class.
        /// </summary>
        /// <param name="supportLoggerService">The support logger service.</param>
        public SettingsService(ISupportLoggerService supportLoggerService)
        {
            _supportLoggerService = supportLoggerService;
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Gets the settings.
        /// </summary>
        /// <returns>
        /// Returns the settings
        /// </returns>
        /// <exception cref="System.Exception">Failed to parse provided settings (Missing key, no value provided or settings file not found)</exception>
        public APISettings GetSettings()
        {
            try
            {
                var iniSettings = ReadIniFile(_filePath);

                if (iniSettings.Count == 0)
                {
                    throw new Exception("Failed to parse provided settings (Missing key, no value provided or settings file not found)");
                }

                return new APISettings
                {
                    URL = iniSettings["TargetInstanceURL"],
                    Email = iniSettings["Username"],
                    APIKey = iniSettings["APIKey"],
                    ExcludedTaskStatus = ParseExclusionFilter(iniSettings["ExcludedTaskStatus"])
                };
            }
            catch (Exception ex)
            {
                _supportLoggerService.AddLog(ex.Message, LogLevel.Error);
                throw;
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Reads the ini file.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>Returns settings parsed from the ini file</returns>
        /// <exception cref="System.IO.FileNotFoundException">The file '{filePath}' was not found.</exception>
        private Dictionary<string, string> ReadIniFile(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    GenerateIniFile();
                    throw new FileNotFoundException($"The settings file 'TaskTickr.ini' was not found in the default path ({filePath}). Generating new ini file in {filePath}");
                }

                var fileContent = File.ReadLines(filePath)
                  .Select(line => line.Split('='))
                  .Select(parts => new
                  {
                      Key = parts[0],
                      Value = string.Join("=", parts.Skip(1))
                  })
                  .ToDictionary(item => item.Key.Trim(), item => item.Value.Trim());

                _supportLoggerService.AddLog("Loaded settings file", LogLevel.Information);
                return fileContent;
            }
            catch (Exception ex)
            {
                _supportLoggerService.AddLog(ex.Message, LogLevel.Error);
                throw;
            }
        }

        /// <summary>
        /// Generates the default ini file.
        /// </summary>
        private void GenerateIniFile()
        {
            string iniFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TaskTickrSettings.ini");

            string iniContent = @"
                    [Settings]
                    TargetInstanceURL = https://instance.atlassian.net
                    Username = user@domain.com
                    APIKey = insert_personal_api_token_key_here";

            File.WriteAllText(iniFilePath, iniContent.Trim());
            _supportLoggerService.AddLog($"Created default ini settings file in {iniFilePath}", LogLevel.Information);
        }

        /// <summary>
        /// Parses the exclusion filter.
        /// </summary>
        /// <param name="exclusionFilter">The exclusion filter.</param>
        /// <returns>Returns the exclusion filters in a Jira supported format</returns>
        private string ParseExclusionFilter(string exclusionFilter)
        {
            var statuses = exclusionFilter.Split(',')
                                .Select(status => status.Trim())
                                .Select(status => $"'{status}'");

            return string.Join(", ", statuses);
        }

    }
    #endregion
}
