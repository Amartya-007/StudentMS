using Microsoft.Extensions.DependencyInjection;
using StudentMS.UI.ViewModels.Students;
using System.Windows;

namespace StudentMS.UI.Views.Students
{
    public partial class StudentFormView : Window
    {
        public StudentFormView(int studentId)
        {
            InitializeComponent();
            var vm = App.ServiceProvider.GetRequiredService<StudentFormViewModel>();
            DataContext = vm;

            vm.SaveCompleted += () =>
            {
                DialogResult = true;
                Close();
            };

            Loaded += async (_, _) => await vm.InitializeCommand.ExecuteAsync(studentId);
        }
    }
}
