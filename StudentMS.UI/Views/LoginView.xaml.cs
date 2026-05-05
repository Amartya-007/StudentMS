using Microsoft.Extensions.DependencyInjection;
using StudentMS.UI.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace StudentMS.UI.Views
{
    public partial class LoginView : Window
    {
        private readonly LoginViewModel _viewModel;
        private bool _syncingPassword;

        public LoginView(LoginViewModel viewModel)
        {
            InitializeComponent();
            _viewModel  = viewModel;
            DataContext = _viewModel;

            // Allow dragging the borderless window
            MouseLeftButtonDown += (_, e) =>
            {
                if (e.ButtonState == MouseButtonState.Pressed)
                    DragMove();
            };

            // PasswordBox → ViewModel (single source of truth for password)
            PasswordBox.PasswordChanged += (_, _) =>
            {
                if (_syncingPassword) return;
                _syncingPassword = true;
                _viewModel.Password = PasswordBox.Password;
                _syncingPassword = false;
            };

            // Keep PasswordBox in sync when ViewModel password changes
            // (e.g. cleared after failed login, or synced from plain TextBox toggle)
            _viewModel.PropertyChanged += (_, e) =>
            {
                if (_syncingPassword) return;
                if (e.PropertyName == nameof(_viewModel.Password))
                {
                    _syncingPassword = true;
                    if (PasswordBox.Password != (_viewModel.Password ?? string.Empty))
                        PasswordBox.Password = _viewModel.Password ?? string.Empty;
                    _syncingPassword = false;
                }

                // When toggling back to hidden mode, re-focus PasswordBox
                if (e.PropertyName == nameof(_viewModel.ShowPassword) && !_viewModel.ShowPassword)
                    PasswordBox.Focus();
            };

            // Navigate to main window on success
            _viewModel.LoginSucceeded += () =>
            {
                var mainWindow = App.ServiceProvider.GetRequiredService<MainWindow>();
                mainWindow.Show();
                Close();
            };

            // Autofocus username on load
            Loaded += (_, _) => UsernameBox.Focus();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
            => Application.Current.Shutdown();

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
            => WindowState = WindowState.Minimized;
    }
}
