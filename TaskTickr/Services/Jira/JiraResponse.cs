namespace TaskTickr.Services.Jira
{
    /// <summary>
    /// Defines the structure of a Jira response object
    /// </summary>
    public class JiraResponse
    {
        /// <summary>
        /// Gets or sets the issues.
        /// </summary>
        /// <value>
        /// The issues.
        /// </value>
        public List<JiraTask> Issues { get; set; }
    }
}
