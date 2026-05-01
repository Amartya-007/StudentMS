using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Serilog;
using StudentMS.Business.Interfaces;
using StudentMS.Models.RequestModels;
using StudentMS.Models.ResponseModels;
using StudentMS.UI.ViewModels.Base;
using System.Collections.ObjectModel;

namespace StudentMS.UI.ViewModels.Departments
{
    public partial class DepartmentListViewModel : BaseViewModel
    {
        private readonly IDepartmentBusiness _departmentBusiness;

        public DepartmentListViewModel(IDepartmentBusiness departmentBusiness)
        {
            _departmentBusiness = departmentBusiness;
        }

        [ObservableProperty] private ObservableCollection<DepartmentResponseModel> _departments = new();
        [ObservableProperty] private DepartmentResponseModel? _selectedDepartment;

        public event Action<int>? OpenFormRequested;

        [RelayCommand]
        private async Task LoadAsync()
        {
            IsBusy = true;
            ClearMessages();
            try
            {
                var result = await _departmentBusiness.GetDepartments(new DepartmentRequestModel());
                if (result.Status)
                    Departments = new ObservableCollection<DepartmentResponseModel>(result.ResponseData ?? new());
                else
                    SetError(result.Message ?? "Failed to load departments.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "DepartmentListViewModel.Load: Error");
                SetError("Unexpected error loading departments.");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand] private void AddDepartment() => OpenFormRequested?.Invoke(0);

        [RelayCommand]
        private void EditDepartment(DepartmentResponseModel dept)
        {
            SelectedDepartment = dept;
            OpenFormRequested?.Invoke(dept.DepartmentId);
        }

        [RelayCommand]
        private async Task DeleteDepartmentAsync(int departmentId)
        {
            IsBusy = true;
            ClearMessages();
            try
            {
                var result = await _departmentBusiness.DeleteDepartment(new DepartmentRequestModel { DepartmentId = departmentId });
                if (result.Status)
                {
                    SetSuccess("Department deleted successfully.");
                    await LoadAsync();
                }
                else
                    SetError(result.Message ?? "Failed to delete department.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "DepartmentListViewModel.Delete: Error");
                SetError("Unexpected error deleting department.");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
