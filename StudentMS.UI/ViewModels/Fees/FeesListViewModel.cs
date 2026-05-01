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
    public partial class FeesListViewModel : BaseViewModel
    {
        private readonly IFeesBusiness    _feesBusiness;
        private readonly IStudentBusiness _studentBusiness;

        public FeesListViewModel(IFeesBusiness feesBusiness, IStudentBusiness studentBusiness)
        {
            _feesBusiness    = feesBusiness;
            _studentBusiness = studentBusiness;
        }

        [ObservableProperty] private ObservableCollection<FeesResponseModel>    _fees     = new();
        [ObservableProperty] private ObservableCollection<StudentResponseModel> _students = new();
        [ObservableProperty] private FeesResponseModel? _selectedFee;
        [ObservableProperty] private int    _filterStudentId;
        [ObservableProperty] private string _filterStatus = "All"; // All / Paid / Pending

        public IReadOnlyList<string> StatusOptions { get; } = new[] { "All", "Paid", "Pending" };

        public event Action<int>? OpenFormRequested;

        [RelayCommand]
        private async Task LoadAsync()
        {
            IsBusy = true;
            ClearMessages();
            try
            {
                var studResult = await _studentBusiness.GetStudents(new StudentRequestModel());
                if (studResult.Status)
                    Students = new ObservableCollection<StudentResponseModel>(studResult.ResponseData ?? new());

                var request = new FeesRequestModel
                {
                    StudentId = FilterStudentId,
                    Status    = FilterStatus == "All" ? null : FilterStatus
                };

                var result = await _feesBusiness.GetFees(request);
                if (result.Status)
                    Fees = new ObservableCollection<FeesResponseModel>(result.ResponseData ?? new());
                else
                    SetError(result.Message ?? "Failed to load fees.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "FeesListViewModel.Load: Error");
                SetError("Unexpected error loading fees.");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand] private void AddFee() => OpenFormRequested?.Invoke(0);

        [RelayCommand]
        private async Task MarkPaidAsync(int feeId)
        {
            IsBusy = true;
            ClearMessages();
            try
            {
                var result = await _feesBusiness.UpdateFeeStatus(new FeesRequestModel { FeeId = feeId, Status = "Paid" });
                if (result.Status)
                {
                    SetSuccess("Fee marked as paid.");
                    await LoadAsync();
                }
                else
                    SetError(result.Message ?? "Failed to update fee status.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "FeesListViewModel.MarkPaid: Error");
                SetError("Unexpected error updating fee.");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
