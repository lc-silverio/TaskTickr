
using TaskTickr.Domain.DTO;

namespace TaskTickr.Domain.Interfaces
{
    /// <summary>
    /// Defines methods that handle Jira related events
    /// </summary>
    public interface IJiraService
    {
        /// <summary>
        /// Gets the task names.
        /// </summary>
        /// <param name="targetEndpoint">The target endpoint.</param>
        /// <param name="filterQuery">The filter query.</param>
        /// <returns>Returns a list of task names</returns>
        Task<List<string>> GetTaskNames(string targetEndpoint, string filterQuery);

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
