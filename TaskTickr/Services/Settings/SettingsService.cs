using System.IO;

namespace TaskTickr.Services.Settings
{
    /// <summary>
    /// Defines methods responsive for handling the processing of settings
    /// </summary>
    /// <seealso cref="TaskTickr.Services.Settings.ISettingsService" />
    public class SettingsService : ISettingsService
    {
        /// <summary>
        /// The ini settings file path
        /// </summary>
        private readonly String _filePath = $"{AppDomain.CurrentDomain.BaseDirectory}TaskTickrSettings.ini";

        /// <summary>
        /// Gets the settings.
        /// </summary>
        /// <returns>
        /// Returns the settings
        /// </returns>
        public APISettings GetSettings()
        {
            var iniSettings = ReadIniFile(_filePath);

            return new APISettings
            {
                TargetInstanceURL = iniSettings["TargetInstanceURL"],
                UserName = iniSettings["Username"],
                Password = iniSettings["Password"]
            };
        }

        /// <summary>
        /// Reads the ini file.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>Returns settings parsed from the ini file</returns>
        /// <exception cref="System.IO.FileNotFoundException">The file '{filePath}' was not found.</exception>
        private static Dictionary<string, string> ReadIniFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"The file '{filePath}' was not found.");
            }

            var fileContent = File.ReadLines(filePath)
              .Select(line => line.Split('='))
              .Select(parts => new
              {
                  Key = parts[0],
                  Value = string.Join("=", parts.Skip(1))
              })
              .ToDictionary(item => item.Key.Trim(), item => item.Value.Trim());

            return fileContent;
        }
    }
}
