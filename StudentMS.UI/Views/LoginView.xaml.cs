using Microsoft.Extensions.DependencyInjection;
using StudentMS.UI.ViewModels;
using System.Windows;

namespace StudentMS.UI.Views
{
    public partial class LoginView : Window
    {
        private readonly LoginViewModel _viewModel;
        private bool _syncingPassword; // guard against recursive updates

        public LoginView(LoginViewModel viewModel)
        {
            InitializeComponent();
            _viewModel  = viewModel;
            DataContext = _viewModel;

            // ── Password sync ────────────────────────────────────────────────────
            // PasswordBox → ViewModel (masked mode)
            PasswordBox.PasswordChanged += (_, _) =>
            {
                if (_syncingPassword) return;
                _syncingPassword = true;
                _viewModel.Password = PasswordBox.Password;
                _syncingPassword = false;
            };

            // PlainPasswordBox → PasswordBox (plain-text mode)
            // The TextBox is two-way bound to ViewModel.Password already,
            // but we also keep PasswordBox in sync so toggling back works.
            _viewModel.PropertyChanged += (_, e) =>
            {
                if (_syncingPassword) return;
                if (e.PropertyName == nameof(_viewModel.Password))
                {
                    _syncingPassword = true;
                    if (PasswordBox.Password != _viewModel.Password)
                        PasswordBox.Password = _viewModel.Password ?? string.Empty;
                    _syncingPassword = false;
                }
            };

            // ── Navigation ───────────────────────────────────────────────────────
            _viewModel.LoginSucceeded += () =>
            {
                var mainWindow = App.ServiceProvider.GetRequiredService<MainWindow>();
                mainWindow.Show();
                Close();
            };

            // Focus username on load
            Loaded += (_, _) => UsernameBox.Focus();
        }
    }
}
