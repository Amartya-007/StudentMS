using CommunityToolkit.Mvvm.ComponentModel;

namespace StudentMS.UI.ViewModels.Base
{
    /// <summary>
    /// Base ViewModel providing common properties: IsBusy, ErrorMessage, SuccessMessage.
    /// All ViewModels inherit from this.
    /// </summary>
    public partial class BaseViewModel : ObservableObject
    {
        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private string? _errorMessage;

        [ObservableProperty]
        private string? _successMessage;

        protected void SetSuccess(string message)
        {
            SuccessMessage = message;
            ErrorMessage   = null;
        }

        protected void SetError(string message)
        {
            ErrorMessage   = message;
            SuccessMessage = null;
        }

        protected void ClearMessages()
        {
            ErrorMessage   = null;
            SuccessMessage = null;
        }
    }
}
