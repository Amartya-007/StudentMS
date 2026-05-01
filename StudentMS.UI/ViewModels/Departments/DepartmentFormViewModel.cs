using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Serilog;
using StudentMS.Business.Interfaces;
using StudentMS.Models.RequestModels;
using StudentMS.UI.ViewModels.Base;

namespace StudentMS.UI.ViewModels.Departments
{
    public partial class DepartmentFormViewModel : BaseViewModel
    {
        private readonly IDepartmentBusiness _departmentBusiness;

        public DepartmentFormViewModel(IDepartmentBusiness departmentBusiness)
        {
            _departmentBusiness = departmentBusiness;
        }

        [ObservableProperty] private int     _departmentId;
        [ObservableProperty] private string? _departmentName;
        [ObservableProperty] private string  _formTitle  = "Add Department";
        [ObservableProperty] private bool    _isEditMode;

        public event Action? SaveCompleted;

        [RelayCommand]
        private async Task InitializeAsync(int departmentId)
        {
            IsBusy = true;
            ClearMessages();
            try
            {
                if (departmentId > 0)
                {
                    IsEditMode = true;
                    FormTitle  = "Edit Department";
                    var result = await _departmentBusiness.GetDepartmentById(new DepartmentRequestModel { DepartmentId = departmentId });
                    if (result.Status && result.ResponseData != null)
                    {
                        DepartmentId   = result.ResponseData.DepartmentId;
                        DepartmentName = result.ResponseData.DepartmentName;
                    }
                }
                else
                {
                    IsEditMode     = false;
                    FormTitle      = "Add Department";
                    DepartmentName = null;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "DepartmentFormViewModel.Initialize: Error");
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
                var request = new DepartmentRequestModel
                {
                    DepartmentId   = DepartmentId,
                    DepartmentName = DepartmentName
                };

                // UpdateDepartment returns AppResult<bool>, InsertDepartment returns AppResult<int>
                // — handle separately to avoid ternary type mismatch
                bool success;
                string? message;

                if (IsEditMode)
                {
                    var updateResult = await _departmentBusiness.UpdateDepartment(request);
                    success = updateResult.Status;
                    message = updateResult.Message;
                }
                else
                {
                    var insertResult = await _departmentBusiness.InsertDepartment(request);
                    success = insertResult.Status;
                    message = insertResult.Message;
                }

                if (success)
                {
                    SetSuccess(IsEditMode ? "Department updated successfully." : "Department added successfully.");
                    SaveCompleted?.Invoke();
                }
                else
                    SetError(message ?? "Failed to save department.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "DepartmentFormViewModel.Save: Error");
                SetError("Unexpected error saving department.");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
