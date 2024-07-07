
namespace TaskTickr.Services.Jira
{
    /// <summary>
    /// Defines methods to be implemented by classes that handle Jira related events
    /// </summary>
    public interface IJiraService
    {
        /// <summary>
        /// Gets the user tasks.
        /// </summary>
        /// <returns>Returns the users tasks</returns>
        Task<IEnumerable<JiraTask>> GetUserTasks();

        /// <summary>
        /// Logs the task time.
        /// </summary>
        /// <param name="taskId">The task identifier.</param>
        /// <param name="timeElapsed">The time elapsed.</param>
        Task LogTaskTime(string taskId, TimeSpan timeElapsed);
    }
}
