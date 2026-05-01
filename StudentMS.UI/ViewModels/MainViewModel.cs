using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using StudentMS.UI.ViewModels.Base;
using StudentMS.UI.ViewModels.Departments;
using StudentMS.UI.ViewModels.Fees;
using StudentMS.UI.ViewModels.Students;
using StudentMS.UI.ViewModels.Teachers;

namespace StudentMS.UI.ViewModels
{
    /// <summary>
    /// Shell ViewModel — owns the sidebar navigation and current page.
    /// </summary>
    public partial class MainViewModel : BaseViewModel
    {
        private readonly IServiceProvider _serviceProvider;

        public MainViewModel(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            // Default page on startup
            NavigateTo("Students");
        }

        [ObservableProperty]
        private BaseViewModel? _currentViewModel;

        [ObservableProperty]
        private string _currentPageTitle = "Students";

        [RelayCommand]
        private void NavigateTo(string page)
        {
            CurrentPageTitle = page;
            CurrentViewModel = page switch
            {
                "Students"    => _serviceProvider.GetRequiredService<StudentListViewModel>(),
                "Teachers"    => _serviceProvider.GetRequiredService<TeacherListViewModel>(),
                "Departments" => _serviceProvider.GetRequiredService<DepartmentListViewModel>(),
                "Fees"        => _serviceProvider.GetRequiredService<FeesListViewModel>(),
                _             => _serviceProvider.GetRequiredService<StudentListViewModel>()
            };
        }
    }
}
