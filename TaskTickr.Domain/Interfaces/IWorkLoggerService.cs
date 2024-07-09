namespace TaskTickr.Domain.Interfaces
{
    /// <summary>
    /// Defines methods related with the logging of work time
    /// </summary>
    public interface IWorkLoggerService
    {
        /// <summary>
        /// Logs the elapsed task time
        /// </summary>
        /// <param name="taskName">Name of the task.</param>
        /// <param name="elapsedTaskTime">The time span between start and end of the timer</param>
        void LogWork(string taskName, TimeSpan elapsedTaskTime);
    }
}
