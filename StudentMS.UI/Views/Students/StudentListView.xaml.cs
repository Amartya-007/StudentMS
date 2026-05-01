using StudentMS.UI.ViewModels.Students;
using System.Windows;
using System.Windows.Controls;

namespace StudentMS.UI.Views.Students
{
    public partial class StudentListView : UserControl
    {
        public StudentListView()
        {
            InitializeComponent();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is StudentListViewModel vm)
            {
                vm.OpenFormRequested += OpenStudentForm;
                await vm.LoadCommand.ExecuteAsync(null);
            }
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
