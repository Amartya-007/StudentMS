using StudentMS.UI.ViewModels.Students;
using System.Windows;
using System.Windows.Controls;

namespace StudentMS.UI.Views.Students
{
    public partial class StudentListView : UserControl
    {
        private bool _initialized;

        public StudentListView()
        {
            InitializeComponent();
            // Guard: if DataContext is already set before Loaded fires, initialize immediately
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
            if (DataContext is not StudentListViewModel vm) return;

            _initialized = true;
            vm.OpenFormRequested += OpenStudentForm;
            await vm.LoadCommand.ExecuteAsync(null);
        }

        private void OpenStudentForm(int studentId)
        {
            var form = new StudentFormView(studentId);
            form.Owner = Window.GetWindow(this);
            if (form.ShowDialog() == true && DataContext is StudentListViewModel vm)
                _ = vm.LoadCommand.ExecuteAsync(null);
        }
    }
}
