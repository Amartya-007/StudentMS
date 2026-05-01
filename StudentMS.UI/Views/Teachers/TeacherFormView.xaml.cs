using Microsoft.Extensions.DependencyInjection;
using StudentMS.UI.ViewModels.Teachers;
using System.Windows;

namespace StudentMS.UI.Views.Teachers
{
    public partial class TeacherFormView : Window
    {
        public TeacherFormView(int teacherId)
        {
            InitializeComponent();
            var vm = App.ServiceProvider.GetRequiredService<TeacherFormViewModel>();
            DataContext = vm;
            vm.SaveCompleted += () => { DialogResult = true; Close(); };
            Loaded += async (_, _) => await vm.InitializeCommand.ExecuteAsync(teacherId);
        }
    }
}
