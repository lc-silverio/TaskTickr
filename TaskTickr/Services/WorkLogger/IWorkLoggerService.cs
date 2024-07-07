namespace TaskTickr.Services.WorkLogger
{
    public interface IWorkLoggerService
    {
        /// <summary>
        /// Logs the elapsed task time
        /// </summary>
        /// <param name="taskName">Name of the task.</param>
        /// <param name="elapsedTaskTime">The time span between start and end of work time</param>
        void LogWork(string taskName, TimeSpan elapsedTaskTime);
    }
}
