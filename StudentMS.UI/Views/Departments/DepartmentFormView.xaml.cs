using Microsoft.Extensions.DependencyInjection;
using StudentMS.UI.ViewModels.Departments;
using System.Windows;

namespace StudentMS.UI.Views.Departments
{
    public partial class DepartmentFormView : Window
    {
        public DepartmentFormView(int departmentId)
        {
            InitializeComponent();
            var vm = App.ServiceProvider.GetRequiredService<DepartmentFormViewModel>();
            DataContext = vm;
            vm.SaveCompleted += () => { DialogResult = true; Close(); };
            Loaded += async (_, _) => await vm.InitializeCommand.ExecuteAsync(departmentId);
        }
    }
}
