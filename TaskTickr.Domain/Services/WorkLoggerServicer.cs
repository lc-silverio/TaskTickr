using Serilog;
using Serilog.Events;
using System.IO;
using TaskTickr.Domain.Interfaces;

namespace TaskTickr.Domain.Services
{
    /// <summary>
    /// Defines methods related with logging of work hours
    /// </summary>
    public class WorkLoggerService : IWorkLoggerService
    {
        #region Properties        
        /// <summary>
        /// The logger
        /// </summary>
        private readonly Serilog.Core.Logger _logger;

        /// <summary>
        /// The log file path
        /// </summary>
        private readonly string logFilePath = $"{AppDomain.CurrentDomain.BaseDirectory}\\logs\\worklog.csv";
        #endregion

        #region Constructor        
        /// <summary>
        /// Initializes a new instance of the <see cref="WorkLoggerService"/> class.
        /// </summary>
        public WorkLoggerService()
        {
            _logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.File(logFilePath,
                          outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss},{Message:lj}{NewLine}",
                          restrictedToMinimumLevel: LogEventLevel.Information,
                          retainedFileCountLimit: null)
            .CreateLogger();

            // Create a new file and add headers
            if (!File.Exists(logFilePath))
            {
                File.AppendAllText(logFilePath, "Timestamp,TaskName,ElapsedTime\n");
            }
        }
        #endregion

        #region Public Methods      

        /// <summary>
        /// Logs the elapsed task time
        /// </summary>
        /// <param name="taskName">Name of the task.</param>
        /// <param name="elapsedTaskTime">The time span between start and end of the timer</param>
        public void LogWork(string taskName, TimeSpan elapsedTaskTime)
        {
            _logger.Information("{TaskName},{ElapsedTime}", taskName, elapsedTaskTime.TotalMinutes);
        }
        #endregion
    }
}
