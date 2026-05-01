using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using StudentMS.Business.Interfaces;
using StudentMS.Business.Services;
using StudentMS.Common.CommonInfra;
using StudentMS.Infrastructure.Interfaces;
using StudentMS.Infrastructure.Services;
using StudentMS.UI.ViewModels;
using StudentMS.UI.ViewModels.Departments;
using StudentMS.UI.ViewModels.Fees;
using StudentMS.UI.ViewModels.Students;
using StudentMS.UI.ViewModels.Teachers;
using StudentMS.UI.Views;
using System.IO;
using System.Windows;

namespace StudentMS.UI
{
    public partial class App : Application
    {
        private IServiceProvider _serviceProvider = null!;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // 1. Build configuration
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .Build();

            // 2. Initialize connection strings
            ConnectionStrings.Initialize(config);

            // 3. Setup Serilog
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(config)
                .CreateLogger();

            Log.Information("StudentMS starting up");

            // 4. Register DI services
            var services = new ServiceCollection();
            RegisterServices(services);
            _serviceProvider = services.BuildServiceProvider();

            // 5. Show login window
            var loginView = _serviceProvider.GetRequiredService<LoginView>();
            loginView.Show();
        }

        private static void RegisterServices(IServiceCollection services)
        {
            // Infrastructure
            services.AddScoped<IStudentInfra, StudentInfra>();
            services.AddScoped<ITeacherInfra, TeacherInfra>();
            services.AddScoped<IDepartmentInfra, DepartmentInfra>();
            services.AddScoped<IFeesInfra, FeesInfra>();
            services.AddScoped<IUserInfra, UserInfra>();

            // Business
            services.AddScoped<IStudentBusiness, StudentBusiness>();
            services.AddScoped<ITeacherBusiness, TeacherBusiness>();
            services.AddScoped<IDepartmentBusiness, DepartmentBusiness>();
            services.AddScoped<IFeesBusiness, FeesBusiness>();
            services.AddScoped<IUserBusiness, UserBusiness>();

            // ViewModels
            services.AddTransient<LoginViewModel>();
            services.AddTransient<MainViewModel>();
            services.AddTransient<StudentListViewModel>();
            services.AddTransient<StudentFormViewModel>();
            services.AddTransient<TeacherListViewModel>();
            services.AddTransient<TeacherFormViewModel>();
            services.AddTransient<DepartmentListViewModel>();
            services.AddTransient<DepartmentFormViewModel>();
            services.AddTransient<FeesListViewModel>();
            services.AddTransient<FeesFormViewModel>();

            // Views
            services.AddTransient<LoginView>();
            services.AddTransient<MainWindow>();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Log.Information("StudentMS shutting down");
            Log.CloseAndFlush();
            base.OnExit(e);
        }
    }
}
