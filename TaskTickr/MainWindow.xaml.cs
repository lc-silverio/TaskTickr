using System.Windows;
using TaskTickr.Domain.Interfaces;
using TaskTickr.Shared.Enums;
using TaskTickr.Properties;
using TaskTickr.Domain.DTO;
using System.Windows.Media;
using System.Timers;
using System.ComponentModel;

namespace TaskTickr
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Properties
        /// <summary>
        /// The logger handler
        /// </summary>
        private readonly ISupportLoggerService _supportLoggerService;

        /// <summary>
        /// The work logger
        /// </summary>
        private readonly IWorkLoggerService _workLoggerService;

        /// <summary>
        /// The settings service
        /// </summary>
        private readonly ISettingsService _settingsService;

        /// <summary>
        /// The jira service
        /// </summary>
        private readonly IJiraService _jiraService;

        /// <summary>
        /// The tasks
        /// </summary>
        private IEnumerable<JiraTask> _tasks = [];

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

        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow" /> class.
        /// </summary>
        /// <param name="jiraService">The jira service.</param>
        /// <param name="supportLoggerService">The support logger service.</param>
        /// <param name="workLoggerService">The work logger service.</param>
        /// <param name="settingsService">The settings service.</param>
        public MainWindow(IJiraService jiraService, ISupportLoggerService supportLoggerService, IWorkLoggerService workLoggerService, ISettingsService settingsService)
        {
            InitializeComponent();
            SetWindowStartingPosition();

            _jiraService = jiraService;
            _supportLoggerService = supportLoggerService;
            _workLoggerService = workLoggerService;
            _settingsService = settingsService;
            _supportLoggerService.AddLog("Starting TaskTickr", LogLevel.Information);

            _timer = new System.Timers.Timer(1000); // 1 second intervals
            _timer.Elapsed += OnTimedEvent!;

            GetTasks();
            DisplayTrayIcon();
        }
        #endregion

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

        #region Tasks
        /// <summary>
        /// Gets the task names and adds them to the combo-box
        /// </summary>
        private async void GetTasks()
        {
            CleanTasks();

            var settings = _settingsService.GetSettings();
            var taskLabels = await _jiraService.GetTaskNames(Settings.Default.Jira_TaskSearchEndpoint, Settings.Default.Jira_FilterQuery.Replace("STATUS_LIST", settings.ExcludedTaskStatus));

            foreach (var task in taskLabels)
            {
                TaskSelector.Items.Add(task);
            }
        }

        /// <summary>
        /// Removes all existing tasks from the task drop-down selector
        /// </summary>
        private void CleanTasks()
        {
            TaskSelector.Items.Clear();
        }

        /// <summary>
        /// Reloads the tasks.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void ReloadTasks(object sender, RoutedEventArgs e)
        {
            GetTasks();
        }
        #endregion

        #region Timer
        /// <summary>
        /// Starts the timer.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <exception cref="System.ArgumentException">No task has been selected</exception>
        private void Start(object sender, EventArgs e)
        {
            if (TaskSelector.SelectedItem == null)
            {
                System.Windows.MessageBox.Show("Failed to start timer. No task has been selected", "Warning!");
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
        /// <param name="e">The e.</param>
        /// <exception cref="System.Exception">Cannot log less than 1 work minute</exception>
        private void Stop(object sender, EventArgs e)
        {
            if (_timer == null || !_timer.Enabled)
            {
                System.Windows.MessageBox.Show("Failed to stop timer. No timer is running.", "Warning");
                _supportLoggerService.AddLog("Attempted to stop a timer that was not started.", LogLevel.Error);
                return;
            }

            _timer.Stop();
            _supportLoggerService.AddLog($"Timer stopped for task {TaskSelector.SelectedValue}", LogLevel.Information);

            if (_elapsedTime.TotalSeconds < 60)
            {
                System.Windows.MessageBox.Show("Cannot log less than 1 work minute", "Warning");
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
            System.Windows.MessageBox.Show("Task time logged successfully", "Good work!");
        }
        #endregion

        #region User controls

        /// <summary>
        /// Disables the user controls.
        /// </summary>
        private void EnableUserControls()
        {
            StartTimer.IsEnabled = true;
            StopTimer.IsEnabled = false;
            TaskSelector.IsEnabled = true;
            Reload.IsEnabled = true;
            TaskSelector.Background = System.Windows.Media.Brushes.Transparent;
        }

        /// <summary>
        /// Enables the user controls.
        /// </summary>
        private void DisableUserControls()
        {
            StartTimer.IsEnabled = false;
            StopTimer.IsEnabled = true;
            TaskSelector.IsEnabled = false;
            Reload.IsEnabled = false;
            TaskSelector.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#E6E6E6"))!;
        }

        /// <summary>
        /// Displays the tray icon.
        /// </summary>
        private void DisplayTrayIcon()
        {
            var _notifyIcon = new NotifyIcon
            {
                Icon = new Icon("icon.ico"),
                Visible = true,
                Text = "TaskTickr",
                ContextMenuStrip = new ContextMenuStrip(),
            };

            _notifyIcon.ContextMenuStrip.Items.Add("Open TaskTickr", null, ShowApplication);
            _notifyIcon.ContextMenuStrip.Items.Add("Start timer", null, Start);
            _notifyIcon.ContextMenuStrip.Items.Add("Stop timer", null, Stop);
            _notifyIcon.ContextMenuStrip.Items.Add("Exit TaskTickr", null, (sender, args) => System.Windows.Application.Current.Shutdown());

            _notifyIcon.DoubleClick += ShowApplication;
        }

        private void ShowApplication(object? sender, EventArgs eventArgs)
        {
            Show();
            WindowState = WindowState.Normal;
        }
        #endregion

        #region Events
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

        /// <summary>
        /// Prevents the user from closing the application without stopping the timer
        /// </summary>
        /// <param name="eventData">The <see cref="CancelEventArgs"/> instance containing the event data.</param>
        protected override void OnClosing(CancelEventArgs eventData)
        {
            if (_timer.Enabled)
            {
                var result = System.Windows.MessageBox.Show("The timer is still running. Are you sure you want to exit?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.No)
                {
                    eventData.Cancel = true;
                    return;
                }

                Dispatcher.Invoke(() => Stop(null, null));
            }

            base.OnClosing(eventData);
        }

        // Override OnStateChanged to minimize to tray instead of task bar.
        /// </summary>
        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
            {
                Hide();
            }
            base.OnStateChanged(e);
        }
        #endregion
    }
}
