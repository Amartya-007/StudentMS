using StudentMS.UI.ViewModels.Fees;
using System.Windows;
using System.Windows.Controls;

namespace StudentMS.UI.Views.Fees
{
    public partial class FeesListView : UserControl
    {
        public FeesListView()
        {
            InitializeComponent();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is FeesListViewModel vm)
            {
                vm.OpenFormRequested += OpenFeesForm;
                await vm.LoadCommand.ExecuteAsync(null);
            }
        }

        private void OpenFeesForm(int feeId)
        {
            var form = new FeesFormView(feeId) { Owner = Window.GetWindow(this) };
            if (form.ShowDialog() == true && DataContext is FeesListViewModel vm)
                _ = vm.LoadCommand.ExecuteAsync(null);
        }
    }
}
