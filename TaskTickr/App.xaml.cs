using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Windows;
using TaskTickr.Domain.Interfaces;
using TaskTickr.Domain.Services;

namespace TaskTickr
{
    public partial class App : System.Windows.Application
    {
        /// <summary>
        /// Gets the application host.
        /// </summary>
        /// <value>
        /// The application host.
        /// </value>
        public static IHost? AppHost { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="App"/> class.
        /// </summary>
        public App()
        {
            AppHost = Host.CreateDefaultBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton<MainWindow>();
                    services.AddSingleton<IWorkLoggerService, WorkLoggerService>();
                    services.AddSingleton<ISupportLoggerService, SupportLoggerService>();
                    services.AddSingleton<ISettingsService, SettingsService>();
                    services.AddTransient<IJiraService, JiraService>();
                })
                .Build();
        }

        /// <summary>
        /// Starts the application
        /// Raises the <see cref="E:System.Windows.Application.Startup" /> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.StartupEventArgs" /> that contains the event data.</param>
        protected override async void OnStartup(StartupEventArgs e)
        {
            await AppHost!.StartAsync(); //! symbol overrides null check as the app host is never null

            // Open starting page/form
            var startPoint = AppHost.Services.GetRequiredService<MainWindow>();
            startPoint.Show();

            // Global exception handler
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            DispatcherUnhandledException += App_DispatcherUnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException!;

            base.OnStartup(e);
        }

        /// <summary>
        /// Stop the app host on exit application
        /// Raises the <see cref="E:System.Windows.Application.Exit" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.Windows.ExitEventArgs" /> that contains the event data.</param>
        protected override async void OnExit(ExitEventArgs e)
        {
            await AppHost!.StopAsync();
            base.OnExit(e);
        }

        /// <summary>
        /// Handles the UnobservedTaskException event of the TaskScheduler control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="UnobservedTaskExceptionEventArgs"/> instance containing the event data.</param>
        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            DisplayErrorMessage(e.Exception);
            e.SetObserved();
        }

        /// <summary>
        /// Handles the DispatcherUnhandledException event of the App control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Threading.DispatcherUnhandledExceptionEventArgs"/> instance containing the event data.</param>
        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            DisplayErrorMessage(e.Exception);
            e.Handled = true;
        }

        /// <summary>
        /// Handles the UnhandledException event of the CurrentDomain control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="UnhandledExceptionEventArgs"/> instance containing the event data.</param>
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            DisplayErrorMessage(e.ExceptionObject as Exception);
        }

        /// <summary>
        /// Display exception message to the user
        /// </summary>
        /// <param name="ex">The ex.</param>
        private void DisplayErrorMessage(Exception ex)
        {
            if (ex != null)
            {
                // Log the exception if necessary
                // LogException(ex);

                System.Windows.MessageBox.Show(ex.Message, "Warning");
            }
        }
    }

}
