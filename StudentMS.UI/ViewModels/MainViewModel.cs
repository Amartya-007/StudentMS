using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using StudentMS.Business.Interfaces;
using StudentMS.Common.Logging;
using StudentMS.Common.Session;
using StudentMS.UI.ViewModels.Base;
using StudentMS.UI.ViewModels.Departments;
using StudentMS.UI.ViewModels.Fees;
using StudentMS.UI.ViewModels.Students;
using StudentMS.UI.ViewModels.Teachers;
using StudentMS.UI.ViewModels.Users;
using StudentMS.UI.Views;
using System.Windows;

namespace StudentMS.UI.ViewModels
{
    /// <summary>
    /// Shell ViewModel — owns the sidebar navigation and current page.
    /// Exposes role-based visibility properties derived from AppSession.
    /// </summary>
    public partial class MainViewModel : BaseViewModel
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IActivityLogBusiness _logger;

        public MainViewModel(IServiceProvider serviceProvider, IActivityLogBusiness logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            // Default page on startup
            NavigateTo("Students");
        }

        [ObservableProperty]
        private BaseViewModel? _currentViewModel;

        [ObservableProperty]
        private string _currentPageTitle = "Students";

        // ── Role helpers ─────────────────────────────────────────────────────

        public bool IsAdmin   => AppSession.Current.IsAdmin;
        public bool IsTeacher => AppSession.Current.IsTeacher;

        // ── Sidebar visibility (Req 3.1 / 3.2) ──────────────────────────────

        /// <summary>Teachers, Departments, and User Management are Admin-only.</summary>
        public Visibility AdminOnlyVisibility => IsAdmin ? Visibility.Visible : Visibility.Collapsed;

        /// <summary>User Management is Admin-only.</summary>
        public Visibility UserMgmtVisibility  => IsAdmin ? Visibility.Visible : Visibility.Collapsed;

        // ── Navigation ───────────────────────────────────────────────────────

        [RelayCommand]
        private void NavigateTo(string page)
        {
            // Req 8.5 / 10.7: Guard — non-Admin cannot navigate to UserManagement
            if (page == "UserManagement" && !IsAdmin)
            {
                Log.Warning("MainViewModel.NavigateTo: Non-admin attempted to navigate to UserManagement");
                SetError("Access denied.");
                return;
            }

            CurrentPageTitle = page;
            CurrentViewModel = page switch
            {
                "Students"       => _serviceProvider.GetRequiredService<StudentListViewModel>(),
                "Teachers"       => _serviceProvider.GetRequiredService<TeacherListViewModel>(),
                "Departments"    => _serviceProvider.GetRequiredService<DepartmentListViewModel>(),
                "Fees"           => _serviceProvider.GetRequiredService<FeesListViewModel>(),
                "UserManagement" => _serviceProvider.GetRequiredService<UserManagementViewModel>(),
                _                => _serviceProvider.GetRequiredService<StudentListViewModel>()
            };
        }

        [RelayCommand]
        private void NavigateToChangePassword()
        {
            CurrentPageTitle = "Change Password";
            CurrentViewModel = _serviceProvider.GetRequiredService<ChangePasswordViewModel>();
        }

        // ── Logout ───────────────────────────────────────────────────────────

        [RelayCommand]
        private void Logout()
        {
            // Req 10.13, 2.3: Log the logout activity before clearing the session
            _logger.LogActivity(ActivityActions.Logout, $"User '{AppSession.Current.Username}' logged out");
            AppSession.Current.ClearSession();

            var loginView = _serviceProvider.GetRequiredService<LoginView>();
            loginView.Show();

            // Close the current MainWindow
            foreach (Window window in Application.Current.Windows)
            {
                if (window is MainWindow)
                {
                    window.Close();
                    break;
                }
            }
        }
    }
}
