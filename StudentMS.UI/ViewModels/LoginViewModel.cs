using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Serilog;
using StudentMS.Business.Interfaces;
using StudentMS.Models.RequestModels;
using StudentMS.UI.ViewModels.Base;

namespace StudentMS.UI.ViewModels
{
    public partial class LoginViewModel : BaseViewModel
    {
        private readonly IUserBusiness _userBusiness;

        public LoginViewModel(IUserBusiness userBusiness)
        {
            _userBusiness = userBusiness;
        }

        [ObservableProperty]
        private string? _username;

        [ObservableProperty]
        private string? _password;

        /// <summary>
        /// When true the View swaps PasswordBox for a plain TextBox so the user can read their password.
        /// </summary>
        [ObservableProperty]
        private bool _showPassword;

        // Field-level validation messages shown inline under each field
        [ObservableProperty]
        private string? _usernameError;

        [ObservableProperty]
        private string? _passwordError;

        /// <summary>Raised when login succeeds — the View subscribes and opens MainWindow.</summary>
        public event Action? LoginSucceeded;

        [RelayCommand]
        private void ToggleShowPassword() => ShowPassword = !ShowPassword;

        [RelayCommand]
        private async Task LoginAsync()
        {
            // Clear previous field errors
            UsernameError = null;
            PasswordError = null;
            ClearMessages();

            // Field-level validation
            bool valid = true;

            if (string.IsNullOrWhiteSpace(Username))
            {
                UsernameError = "Username is required.";
                valid = false;
            }

            if (string.IsNullOrWhiteSpace(Password))
            {
                PasswordError = "Password is required.";
                valid = false;
            }

            if (!valid) return;

            IsBusy = true;
            try
            {
                var result = await _userBusiness.ValidateUser(new UserRequestModel
                {
                    Username = Username,
                    Password = Password
                });

                if (result.Status)
                {
                    Log.Information("Login succeeded for user: {Username}", Username);
                    LoginSucceeded?.Invoke();
                }
                else
                {
                    // Show a generic message at the form level (don't reveal which field is wrong)
                    SetError(result.Message ?? "Invalid username or password.");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "LoginViewModel.Login: Error");
                SetError("An unexpected error occurred. Please try again.");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
