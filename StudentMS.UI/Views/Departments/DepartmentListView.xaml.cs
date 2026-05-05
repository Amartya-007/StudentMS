using StudentMS.UI.ViewModels.Departments;
using System.Windows;
using System.Windows.Controls;

namespace StudentMS.UI.Views.Departments
{
    public partial class DepartmentListView : UserControl
    {
        private bool _initialized;

        public DepartmentListView()
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            await TryInitializeAsync();
        }

        private async void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            await TryInitializeAsync();
        }

        private async Task TryInitializeAsync()
        {
            if (_initialized) return;
            if (DataContext is not DepartmentListViewModel vm) return;

            _initialized = true;
            vm.OpenFormRequested += OpenDepartmentForm;
            await vm.LoadCommand.ExecuteAsync(null);
        }

        private void OpenDepartmentForm(int departmentId)
        {
            var form = new DepartmentFormView(departmentId) { Owner = Window.GetWindow(this) };
            if (form.ShowDialog() == true && DataContext is DepartmentListViewModel vm)
                _ = vm.LoadCommand.ExecuteAsync(null);
        }
    }
}
