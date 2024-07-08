using System.Windows;
using TaskTickr.Enums;
using TaskTickr.Services.Jira;
using TaskTickr.Services.SupportLogger;
using TaskTickr.Services.WorkLogger;

namespace TaskTickr
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// The logger handler
        /// </summary>
        private readonly ISupportLoggerService _logger;

        /// <summary>
        /// The work logger
        /// </summary>
        private readonly IWorkLoggerService _workLogger;

        /// <summary>
        /// The jira service
        /// </summary>
        private readonly IJiraService _jiraService;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        /// <param name="jiraService">The jira service.</param>
        /// <param name="workLoggerService">The work logger service.</param>
        /// <param name="supportLoggerService">The support logger service.</param>
        public MainWindow(IJiraService jiraService, IWorkLoggerService workLoggerService, ISupportLoggerService supportLoggerService)
        {
            InitializeComponent();
            SetWindowStartingPosition();

            // Logging
            _logger = supportLoggerService;
            _workLogger = workLoggerService;

            _logger.AddLog("Starting TaskTickr", LogLevel.Information);
            _workLogger.LogWork("Test", TimeSpan.FromSeconds(1));

            // Jira
            _jiraService = jiraService;

            _jiraService.GetUserTasks();
            _jiraService.LogTaskTime("1", TimeSpan.FromSeconds(1));
        }

        /// <summary>
        /// Definies starting position of the window on the primary screen
        /// </summary>
        private void SetWindowStartingPosition()
        {
            // Get size of the primary screen
            var screenSize = SystemParameters.WorkArea;

            // Set the window's position to the bottom right corner
            // workingArea.Right - this.Width = (total width of the screen - defined width of the application)
            // workingArea.Bottom - this.Height = (total hight of the screen - defined height of the application
            this.Left = screenSize.Right - this.Width;
            this.Top = screenSize.Bottom - this.Height;
        }
    }
}