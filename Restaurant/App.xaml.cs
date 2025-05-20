using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using System.Data;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Database;
using System.IO;
using Database.Services.Interfaces;
using Restaurant.ViewModels;

namespace Restaurant
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private ServiceProvider _serviceProvider;
        public static IServiceProvider ServiceProvider { get; private set; }
        public static IConfiguration Configuration { get; private set; }

        public App()
        {
            // Build configuration
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            Configuration = builder.Build();

            // Configure services
            var services = new ServiceCollection();
            ConfigureServices(services);

            _serviceProvider = services.BuildServiceProvider();
            ServiceProvider = _serviceProvider;
            System.Diagnostics.PresentationTraceSources.DataBindingSource.Switch.Level = System.Diagnostics.SourceLevels.Critical;
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // Register configuration
            services.AddSingleton<IConfiguration>(Configuration);

            // Register restaurant services (repositories and service classes)
            services.AddRestaurantServices(Configuration);

            // Register JSON services
            services.AddSingleton<JsonFileService>();
            services.AddSingleton<AppSettingsService>();
            services.AddSingleton<DataExportService>();

            // Register app-specific services
            services.AddSingleton<IDataRefreshService, DataRefreshService>();

            // Register ViewModels
            services.AddTransient<FoodDisplayViewModel>();

            // Register Windows
            services.AddTransient<MainWindow>();
            services.AddTransient<AddMenuViewModel>();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                // Test service resolution
                var foodDisplayService = _serviceProvider.GetService<IFoodDisplayService>();
                var menuService = _serviceProvider.GetService<IMenuService>();
                var refreshService = _serviceProvider.GetService<IDataRefreshService>();

                if (foodDisplayService == null) MessageBox.Show("foodDisplayService is null!");
                if (menuService == null) MessageBox.Show("menuService is null!");
                if (refreshService == null) MessageBox.Show("refreshService is null!");

                // Check if ViewModel can be resolved
                var viewModel = _serviceProvider.GetService<FoodDisplayViewModel>();
                if (viewModel == null)
                {
                    MessageBox.Show("Failed to resolve FoodDisplayViewModel!");
                }
                else
                {
                    // Check if commands are initialized
                    if (viewModel.RefreshCommand == null) MessageBox.Show("RefreshCommand is null!");
                    if (viewModel.AddNewPreparatCommand == null) MessageBox.Show("AddNewPreparatCommand is null!");
                }

                var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
                mainWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during startup: {ex.Message}");
            }

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            // Save any settings before closing
            SaveApplicationSettings();

            // Dispose the service provider to clean up resources
            _serviceProvider.Dispose();

            base.OnExit(e);
        }

        private void LoadApplicationSettings()
        {
            try
            {
                // Example of loading app settings
                var settingsService = _serviceProvider.GetService<AppSettingsService>();
                if (settingsService != null)
                {
                    var settings = settingsService.LoadSettings();
                    // Apply settings to application if needed
                    // Example: ApplyTheme(settings.Theme);
                }
            }
            catch (Exception ex)
            {
                // Log error or show message
                MessageBox.Show($"Error loading settings: {ex.Message}", "Settings Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void SaveApplicationSettings()
        {
            try
            {
                // Example of saving app settings
                var settingsService = _serviceProvider.GetService<AppSettingsService>();
                if (settingsService != null)
                {
                    // Get current settings state and save
                    var settings = new AppSettings();
                    // Example: settings.Theme = CurrentTheme;
                    settingsService.SaveSettings(settings);
                }
            }
            catch (Exception ex)
            {
                // Log error or show message
                MessageBox.Show($"Error saving settings: {ex.Message}", "Settings Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}

