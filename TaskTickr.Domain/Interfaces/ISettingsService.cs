using TaskTickr.Domain.DTO;

namespace TaskTickr.Domain.Interfaces
{
    /// <summary>
    /// Defines methods for handling settings
    /// </summary>
    public interface ISettingsService
    {
        /// <summary>
        /// Gets the settings.
        /// </summary>
        /// <returns>Returns the settings</returns>
        public APISettings GetSettings();
    }
}
