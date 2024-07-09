using TaskTickr.Shared.Enums;

namespace TaskTickr.Domain.Interfaces
{
    /// <summary>
    /// Defines methods related with the handling of support related events
    /// </summary>
    public interface ISupportLoggerService
    {
        /// <summary>
        /// Adds a new information to the log
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="logLevel">The log level.</param>
        void AddLog(string message, LogLevel logLevel);
    }
}
