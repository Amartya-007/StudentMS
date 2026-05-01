using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Serilog;
using StudentMS.Business.Interfaces;
using StudentMS.Models.RequestModels;
using StudentMS.UI.ViewModels.Base;
using System.Windows;

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
        /// Raised when login succeeds — the View subscribes and opens MainWindow.
        /// </summary>
        public event Action? LoginSucceeded;

        [RelayCommand]
        private async Task LoginAsync()
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                SetError("Username and password are required.");
                return;
            }

            IsBusy = true;
            ClearMessages();

            try
            {
                var request = new UserRequestModel
                {
                    Username = Username,
                    Password = Password
                };

                var result = await _userBusiness.ValidateUser(request);

                if (result.Status)
                {
                    Log.Information("Login succeeded for user: {Username}", Username);
                    LoginSucceeded?.Invoke();
                }
                else
                {
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
