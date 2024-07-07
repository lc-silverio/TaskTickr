using TaskTickr.Enums;

namespace TaskTickr.Services.SupportLogger
{
    /// <summary>
    /// Defines the interface for the Logger class that other classes should implement
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
