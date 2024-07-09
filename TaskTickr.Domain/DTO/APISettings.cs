namespace TaskTickr.Domain.DTO
{
    /// <summary>
    /// Defines the api settings object
    /// </summary>
    public class APISettings
    {
        /// <summary>
        /// Gets or sets the target instance URL.
        /// </summary>
        /// <value>
        /// The target instance URL.
        /// </value>
        public required string URL { get; set; }

        /// <summary>
        /// Gets or sets the email
        /// </summary>
        /// <value>
        /// The email of the user.
        /// </value>
        public required string Email { get; set; }

        /// <summary>
        /// Gets or sets the API key.
        /// </summary>
        /// <value>
        /// The API key
        /// </value>
        public required string APIKey { get; set; }

        /// <summary>
        /// Gets or sets the task status that should be excluded from the task search
        /// </summary>
        /// <value>
        /// The task status that should be excluded
        /// </value>
        public required string ExcludedTaskStatus { get; set; }
    }
}
