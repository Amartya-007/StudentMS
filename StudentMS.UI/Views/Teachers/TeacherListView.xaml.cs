using StudentMS.UI.ViewModels.Teachers;
using System.Windows;
using System.Windows.Controls;

namespace StudentMS.UI.Views.Teachers
{
    public partial class TeacherListView : UserControl
    {
        private bool _initialized;

        public TeacherListView()
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
            if (DataContext is not TeacherListViewModel vm) return;

            _initialized = true;
            vm.OpenFormRequested += OpenTeacherForm;
            await vm.LoadCommand.ExecuteAsync(null);
        }

        private void OpenTeacherForm(int teacherId)
        {
            var form = new TeacherFormView(teacherId) { Owner = Window.GetWindow(this) };
            if (form.ShowDialog() == true && DataContext is TeacherListViewModel vm)
                _ = vm.LoadCommand.ExecuteAsync(null);
        }
    }
}
