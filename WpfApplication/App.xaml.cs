using WpfApplication.Data;
using WpfApplication.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using WpfApplication.ApplicationWindows;
using Microsoft.Extensions.Logging;

namespace WpfApplication
{
    public partial class App : Application
    {
        public static IServiceProvider? ServiceProvider { get; private set; }
        protected override void OnStartup(StartupEventArgs eventArgs)
        {
            base.OnStartup(eventArgs);
            InitializeContainer();
        }
        private static void InitializeContainer()
        {
            ServiceCollection services = new();
            services.AddDbContext<DataContext>();
            services.AddScoped<ICompaniesDbContext, DataContext>();
            services.AddScoped<IEmloyeesDbContext, DataContext>();
            ServiceProvider = services.BuildServiceProvider();

            ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.ClearProviders();
                builder.AddDebug();
            });
            ILogger<MainWindow> logger = loggerFactory.CreateLogger<MainWindow>();
            MainWindow window = new MainWindow(ServiceProvider, logger);
            window.Show();
        }
    }

}
