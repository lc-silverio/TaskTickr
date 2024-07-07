namespace TaskTickr.Services.Settings
{
    /// <summary>
    /// Defines the settings object
    /// </summary>
    public class APISettings
    {
        /// <summary>
        /// Gets or sets the target instance URL.
        /// </summary>
        /// <value>
        /// The target instance URL.
        /// </value>
        public required string TargetInstanceURL { get; set; }

        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        /// <value>
        /// The name of the user.
        /// </value>
        public required string UserName { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        public required string Password { get; set; }
    }
}
