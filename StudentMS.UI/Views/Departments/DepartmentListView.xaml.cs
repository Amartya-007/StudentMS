using StudentMS.UI.ViewModels.Departments;
using System.Windows;
using System.Windows.Controls;

namespace StudentMS.UI.Views.Departments
{
    public partial class DepartmentListView : UserControl
    {
        public DepartmentListView()
        {
            InitializeComponent();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is DepartmentListViewModel vm)
            {
                vm.OpenFormRequested += OpenDepartmentForm;
                await vm.LoadCommand.ExecuteAsync(null);
            }
        }

        private void OpenDepartmentForm(int departmentId)
        {
            var form = new DepartmentFormView(departmentId) { Owner = Window.GetWindow(this) };
            if (form.ShowDialog() == true && DataContext is DepartmentListViewModel vm)
                _ = vm.LoadCommand.ExecuteAsync(null);
        }
    }
}
