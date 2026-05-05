using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Serilog;
using StudentMS.Business.Interfaces;
using StudentMS.Common.Session;
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
            else
            {
                Username = Username.Trim(); // trim whitespace
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

                if (result.Status && result.ResponseData != null)
                {
                    var user = result.ResponseData;

                    // Req 1.5 / 7.2: Populate AppSession singleton with authenticated user data
                    AppSession.Current.SetSession(
                        userId:    user.UserId,
                        username:  user.Username,
                        role:      user.Role,
                        teacherId: user.TeacherId
                    );

                    Log.Information("Login succeeded for user: {Username} (Role={Role})", user.Username, user.Role);
                    LoginSucceeded?.Invoke();
                }
                else
                {
                    // Clear password field on failure so user can retype cleanly
                    Password = string.Empty;
                    // Show the message returned by the business layer (lockout, inactive, etc.)
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
