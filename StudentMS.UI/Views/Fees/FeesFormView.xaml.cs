using Microsoft.Extensions.DependencyInjection;
using StudentMS.UI.ViewModels.Fees;
using System.Windows;

namespace StudentMS.UI.Views.Fees
{
    public partial class FeesFormView : Window
    {
        public FeesFormView(int feeId)
        {
            InitializeComponent();
            var vm = App.ServiceProvider.GetRequiredService<FeesFormViewModel>();
            DataContext = vm;
            vm.SaveCompleted += () => { DialogResult = true; Close(); };
            Loaded += async (_, _) => await vm.InitializeCommand.ExecuteAsync(feeId);
        }
    }
}
