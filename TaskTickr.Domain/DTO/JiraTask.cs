namespace TaskTickr.Domain.DTO
{
    /// <summary>
    /// Defines the properties of a Jira task
    /// </summary>
    public class JiraTask
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>
        /// The key.
        /// </value>
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the summary.
        /// </summary>
        /// <value>
        /// The summary.
        /// </value>
        public Fields Fields { get; set; }
    }

    /// <summary>
    /// Defines the sub-properties of the Fields property
    /// </summary>
    public class Fields
    {
        /// <summary>
        /// Gets or sets the summary.
        /// </summary>
        /// <value>
        /// The summary.
        /// </value>
        public string Summary { get; set; }
    }
}


