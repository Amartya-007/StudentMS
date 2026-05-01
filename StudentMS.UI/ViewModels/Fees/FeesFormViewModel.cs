using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Serilog;
using StudentMS.Business.Interfaces;
using StudentMS.Models.RequestModels;
using StudentMS.Models.ResponseModels;
using StudentMS.UI.ViewModels.Base;
using System.Collections.ObjectModel;

namespace StudentMS.UI.ViewModels.Fees
{
    public partial class FeesFormViewModel : BaseViewModel
    {
        private readonly IFeesBusiness    _feesBusiness;
        private readonly IStudentBusiness _studentBusiness;

        public FeesFormViewModel(IFeesBusiness feesBusiness, IStudentBusiness studentBusiness)
        {
            _feesBusiness    = feesBusiness;
            _studentBusiness = studentBusiness;
        }

        [ObservableProperty] private int      _feeId;
        [ObservableProperty] private int      _studentId;
        [ObservableProperty] private decimal  _amount;
        [ObservableProperty] private string   _formTitle  = "Add Fee";
        [ObservableProperty] private bool     _isEditMode;

        [ObservableProperty]
        private ObservableCollection<StudentResponseModel> _students = new();

        public event Action? SaveCompleted;

        [RelayCommand]
        private async Task InitializeAsync(int feeId)
        {
            IsBusy = true;
            ClearMessages();
            try
            {
                var studResult = await _studentBusiness.GetStudents(new StudentRequestModel());
                if (studResult.Status)
                    Students = new ObservableCollection<StudentResponseModel>(studResult.ResponseData ?? new());

                IsEditMode = feeId > 0;
                FormTitle  = IsEditMode ? "Edit Fee" : "Add Fee";
                FeeId      = feeId;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "FeesFormViewModel.Initialize: Error");
                SetError("Failed to load form data.");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task SaveAsync()
        {
            IsBusy = true;
            ClearMessages();
            try
            {
                var request = new FeesRequestModel
                {
                    FeeId     = FeeId,
                    StudentId = StudentId,
                    Amount    = Amount,
                    Status    = "Pending"
                };

                var result = await _feesBusiness.InsertFee(request);

                if (result.Status)
                {
                    SetSuccess("Fee added successfully.");
                    SaveCompleted?.Invoke();
                }
                else
                    SetError(result.Message ?? "Failed to save fee.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "FeesFormViewModel.Save: Error");
                SetError("Unexpected error saving fee.");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
