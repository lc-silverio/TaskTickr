using System.Windows;
using TaskTickr.Domain.Interfaces;
using TaskTickr.Shared.Enums;
using TaskTickr.Properties;
using TaskTickr.Domain.DTO;
using System.Windows.Media;
using System.Timers;

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
        private readonly ISupportLoggerService _supportLoggerService;

        /// <summary>
        /// The work logger
        /// </summary>
        private readonly IWorkLoggerService _workLoggerService;

        /// <summary>
        /// The jira service
        /// </summary>
        private readonly IJiraService _jiraService;

        /// <summary>
        /// The tasks
        /// </summary>
        private IEnumerable<JiraTask> _tasks;

        /// <summary>
        /// The start date
        /// </summary>
        private DateTime _startTime;

        /// <summary>
        /// The timer
        /// </summary>
        private readonly System.Timers.Timer _timer;

        /// <summary>
        /// The elapsed time
        /// </summary>
        private TimeSpan _elapsedTime;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow" /> class.
        /// </summary>
        /// <param name="jiraService">The jira service.</param>
        /// <param name="supportLoggerService">The support logger service.</param>
        /// <param name="workLoggerService">The work logger service.</param>
        public MainWindow(IJiraService jiraService, ISupportLoggerService supportLoggerService, IWorkLoggerService workLoggerService)
        {
            InitializeComponent();
            SetWindowStartingPosition();

            _jiraService = jiraService;
            _supportLoggerService = supportLoggerService;
            _workLoggerService = workLoggerService;

            _supportLoggerService.AddLog("Starting TaskTickr", LogLevel.Information);

            _timer = new System.Timers.Timer(1000); // 1 second intervals
            _timer.Elapsed += OnTimedEvent;

            GetTasks();
        }

        /// <summary>
        /// Gets the task names and adds them to the combo-box
        /// </summary>
        private async void GetTasks()
        {
            _tasks = await _jiraService.GetUserTasks(Settings.Default.Jira_TaskSearchEndpoint, Settings.Default.Jira_FilterQuery);
            var taskLabels = _tasks.Select(x => x.Fields.Summary).OrderBy(x => x).ToList();

            foreach (var task in taskLabels)
            {
                TaskSelector.Items.Add(task);
            }
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

        /// <summary>
        /// Starts the timer.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        /// <exception cref="System.ArgumentException">No task has been selected</exception>
        private void Start(object sender, RoutedEventArgs e)
        {
            if (TaskSelector.SelectedItem == null)
            {
                MessageBox.Show("No task has been selected", "Warning!");
                DisableUserControls();
                return;
            }

            // Reset and start timer
            _elapsedTime = TimeSpan.Zero;
            _startTime = DateTime.UtcNow;
            _timer.Start();

            _supportLoggerService.AddLog($"Timer started for task {TaskSelector.SelectedValue}", LogLevel.Information);
            DisableUserControls();
        }

        /// <summary>
        /// Stops the timer.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        /// <exception cref="System.Exception">Cannot log less than 1 work minute</exception>
        private void Stop(object sender, RoutedEventArgs e)
        {
            _timer.Stop();
            _supportLoggerService.AddLog($"Timer stoped for task {TaskSelector.SelectedValue}", LogLevel.Information);

            if (_elapsedTime.TotalSeconds < 60)
            {
                MessageBox.Show("Cannot log less than 1 work minute", "Warning");
                _supportLoggerService.AddLog("Cannot log less than 1 work minute", LogLevel.Error);

                _elapsedTime = TimeSpan.Zero;
                TimerLabel.Text = "00:00:00";
                EnableUserControls();
                return;
            }

            // Log task time
            var taskName = TaskSelector.SelectedValue.ToString()!;
            var taskId = Convert.ToInt32(_tasks.First(x => x.Fields.Summary == taskName).Id);

            _workLoggerService.LogWork(taskName, _elapsedTime);
            _jiraService.LogTaskTime(taskId, taskName, _elapsedTime, _startTime, Settings.Default.Jira_LogTaskTime);

            // Reload tasks
            GetTasks();
            EnableUserControls();
            MessageBox.Show("Task time logged successfully", "Good work!");
        }

        /// <summary>
        /// Enables the user controls.
        /// </summary>
        private void DisableUserControls()
        {
            StartTimer.IsEnabled = false;
            TaskSelector.IsEnabled = false;
            TaskSelector.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#E6E6E6"))!;
        }

        /// <summary>
        /// Disables the user controls.
        /// </summary>
        private void EnableUserControls()
        {
            StartTimer.IsEnabled = true;
            TaskSelector.IsEnabled = true;
            TaskSelector.Background = Brushes.Transparent;
        }

        /// <summary>
        /// Updates the displayed time
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="ElapsedEventArgs"/> instance containing the event data.</param>
        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            _elapsedTime = _elapsedTime.Add(TimeSpan.FromSeconds(1));
            Dispatcher.Invoke(() => TimerLabel.Text = _elapsedTime.ToString(@"hh\:mm\:ss"));
        }
    }

}
