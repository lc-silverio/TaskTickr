using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Windows;
using TaskTickr.Services.Jira;
using TaskTickr.Services.Settings;
using TaskTickr.Services.SupportLogger;
using TaskTickr.Services.WorkLogger;

namespace TaskTickr
{
    public partial class App : Application
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
    }

}
