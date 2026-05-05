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
using StudentMS.UI.ViewModels.Users;
using StudentMS.UI.Views;
using StudentMS.UI.Views.Users;
using System.IO;
using System.Windows;

namespace StudentMS.UI
{
    public partial class App : Application
    {
        // Static accessor so code-behind can resolve services without constructor injection
        public static IServiceProvider ServiceProvider { get; private set; } = null!;

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
            ServiceProvider = services.BuildServiceProvider();

            // 5. Show login window
            var loginView = ServiceProvider.GetRequiredService<LoginView>();
            loginView.Show();
        }

        private static void RegisterServices(IServiceCollection services)
        {
            // Infrastructure
            services.AddScoped<IStudentInfra,    StudentInfra>();
            services.AddScoped<ITeacherInfra,    TeacherInfra>();
            services.AddScoped<IDepartmentInfra, DepartmentInfra>();
            services.AddScoped<IFeesInfra,       FeesInfra>();
            services.AddScoped<IUserInfra,       UserInfra>();
            services.AddScoped<IActivityLogInfra, ActivityLogInfra>();

            // Business
            services.AddScoped<IStudentBusiness,         StudentBusiness>();
            services.AddScoped<ITeacherBusiness,         TeacherBusiness>();
            services.AddScoped<IDepartmentBusiness,      DepartmentBusiness>();
            services.AddScoped<IFeesBusiness,            FeesBusiness>();
            services.AddScoped<IUserBusiness,            UserBusiness>();
            services.AddScoped<IAccountManagerBusiness,  AccountManagerBusiness>();
            services.AddScoped<IActivityLogBusiness,     ActivityLogBusiness>();
            services.AddScoped<IActivityLogQueryBusiness, ActivityLogQueryBusiness>();

            // ViewModels
            // Login/Main are transient — created fresh each session
            services.AddTransient<LoginViewModel>();
            services.AddTransient<MainViewModel>();
            // Form ViewModels are transient — each form open gets a fresh instance
            services.AddTransient<StudentFormViewModel>();
            services.AddTransient<TeacherFormViewModel>();
            services.AddTransient<DepartmentFormViewModel>();
            services.AddTransient<FeesFormViewModel>();
            // Page ViewModels are singleton — created once, cached across tab switches
            // This prevents redundant DB calls and fixes the Loaded timing race
            services.AddSingleton<StudentListViewModel>();
            services.AddSingleton<TeacherListViewModel>();
            services.AddSingleton<DepartmentListViewModel>();
            services.AddSingleton<FeesListViewModel>();
            services.AddSingleton<UserManagementViewModel>();
            services.AddSingleton<ChangePasswordViewModel>();

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
