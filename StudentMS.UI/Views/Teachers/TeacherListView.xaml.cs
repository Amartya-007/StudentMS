using StudentMS.UI.ViewModels.Teachers;
using System.Windows;
using System.Windows.Controls;

namespace StudentMS.UI.Views.Teachers
{
    public partial class TeacherListView : UserControl
    {
        public TeacherListView()
        {
            InitializeComponent();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is TeacherListViewModel vm)
            {
                vm.OpenFormRequested += OpenTeacherForm;
                await vm.LoadCommand.ExecuteAsync(null);
            }
        }

        private void OpenTeacherForm(int teacherId)
        {
            var form = new TeacherFormView(teacherId) { Owner = Window.GetWindow(this) };
            if (form.ShowDialog() == true && DataContext is TeacherListViewModel vm)
                _ = vm.LoadCommand.ExecuteAsync(null);
        }
    }
}
