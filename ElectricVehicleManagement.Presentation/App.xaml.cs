using System.Configuration;
using System.Data;
using System.Windows;
using CloudinaryDotNet;
using ElectricVehicleManagement.Data.Abstraction;
using ElectricVehicleManagement.Data.Implementation;
using ElectricVehicleManagement.Data.Models;
using ElectricVehicleManagement.Service.Cloudinary;
using ElectricVehicleManagement.Service.Listing;
using ElectricVehicleManagement.Service.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ElectricVehicleManagement.Presentation
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static User CurrentUser { get; set; } = null!;
        public static IConfiguration Configuration { get; set; } = null!;
        public static IServiceProvider ServiceProvider { get; private set; } = null!;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var builder = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory) 
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true); 

            Configuration = builder.Build();
            var services = new ServiceCollection();

            ConfigureServices(services);

            ServiceProvider = services.BuildServiceProvider();
            
            using (var scope = ServiceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                dbContext.Database.Migrate(); 
            }
            
            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<IDbContext, ApplicationDbContext>((sp, options) =>
                options
                    .UseNpgsql(
                        Configuration.GetConnectionString("Database"),
                        npgsqlOptions => npgsqlOptions
                            .MigrationsHistoryTable(HistoryRepository.DefaultTableName, "public")));
            
            services.AddSingleton(provider =>
            {
                var cloudName = Configuration["Cloudinary:CloudName"];
                var apiKey = Configuration["Cloudinary:ApiKey"];
                var apiSecret = Configuration["Cloudinary:ApiSecret"];

                return new Cloudinary(new Account(cloudName, apiKey, apiSecret));
            });

            services.AddScoped<ICloudinaryService, CloudinaryService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IListingService, ListingService>();
            services.AddTransient<MainWindow>();
            services.AddTransient<ListingDetailWindow>();

        }
    }
}

