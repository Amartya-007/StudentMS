using StudentMS.UI.ViewModels.Fees;
using System.Windows;
using System.Windows.Controls;

namespace StudentMS.UI.Views.Fees
{
    public partial class FeesListView : UserControl
    {
        private bool _initialized;

        public FeesListView()
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
            if (DataContext is not FeesListViewModel vm) return;

            _initialized = true;
            vm.OpenFormRequested += OpenFeesForm;
            await vm.LoadCommand.ExecuteAsync(null);
        }

        private void OpenFeesForm(int feeId)
        {
            var form = new FeesFormView(feeId) { Owner = Window.GetWindow(this) };
            if (form.ShowDialog() == true && DataContext is FeesListViewModel vm)
                _ = vm.LoadCommand.ExecuteAsync(null);
        }
    }
}
