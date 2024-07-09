
using TaskTickr.Domain.DTO;

namespace TaskTickr.Domain.Interfaces
{
    /// <summary>
    /// Defines methods that handle Jira related events
    /// </summary>
    public interface IJiraService
    {
        /// <summary>
        /// Gets the user tasks.
        /// </summary>
        /// <param name="targetEndpoint">The target endpoint.</param>
        /// <param name="filterQuery">The filter query.</param>
        /// <returns>
        /// Returns the users tasks
        /// </returns>
        Task<IEnumerable<JiraTask>> GetUserTasks(string targetEndpoint, string filterQuery);

        /// <summary>
        /// Logs the task time.
        /// </summary>
        /// <param name="taskId">The task identifier.</param>
        /// <param name="taskName">Name of the task.</param>
        /// <param name="timeElapsed">The time elapsed.</param>
        /// <param name="startDate">The start date.</param>
        /// <param name="targetEndpoint">The target endpoint.</param>
        /// <returns></returns>
        Task LogTaskTime(int taskId, string taskName, TimeSpan timeElapsed, DateTime startDate, string targetEndpoint);
    }
}
