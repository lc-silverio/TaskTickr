using Serilog;
using TaskTickr.Enums;

namespace TaskTickr.Services.SupportLogger
{
    /// <summary>
    /// Defines methods related with logging
    /// </summary>
    public class SupportLoggerService : ISupportLoggerService
    {
        #region Properties
        private readonly Serilog.Core.Logger _logger;
        #endregion

        #region Constructor        
        /// <summary>
        /// Initializes a new instance of the <see cref="SupportLoggerService"/> class.
        /// </summary>
        public SupportLoggerService()
        {
            _logger = new LoggerConfiguration()
                .WriteTo.File("logs/log.txt", 
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} : {Message:lj}{NewLine}")
                .CreateLogger();
        }
        #endregion

        #region Public Methods        
        /// <summary>
        /// Adds a new information to the log
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="logLevel">The log level.</param>
        public void AddLog(string message, LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Error:
                    _logger.Error(message);
                    break;
                case LogLevel.Warning:
                    _logger.Warning(message);
                    break;
                default:
                    _logger.Information(message);
                    break;
            };
        }
        #endregion
    }
}
