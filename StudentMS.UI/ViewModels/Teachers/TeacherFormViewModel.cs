using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Serilog;
using StudentMS.Business.Interfaces;
using StudentMS.Models.RequestModels;
using StudentMS.Models.ResponseModels;
using StudentMS.UI.ViewModels.Base;
using System.Collections.ObjectModel;

namespace StudentMS.UI.ViewModels.Teachers
{
    public partial class TeacherFormViewModel : BaseViewModel
    {
        private readonly ITeacherBusiness    _teacherBusiness;
        private readonly IDepartmentBusiness _departmentBusiness;

        public TeacherFormViewModel(ITeacherBusiness teacherBusiness, IDepartmentBusiness departmentBusiness)
        {
            _teacherBusiness    = teacherBusiness;
            _departmentBusiness = departmentBusiness;
        }

        [ObservableProperty] private int     _teacherId;
        [ObservableProperty] private string? _name;
        [ObservableProperty] private string? _phone;
        [ObservableProperty] private int     _departmentId;
        [ObservableProperty] private string  _formTitle  = "Add Teacher";
        [ObservableProperty] private bool    _isEditMode;

        [ObservableProperty]
        private ObservableCollection<DepartmentResponseModel> _departments = new();

        public event Action? SaveCompleted;

        [RelayCommand]
        private async Task InitializeAsync(int teacherId)
        {
            IsBusy = true;
            ClearMessages();
            try
            {
                var deptResult = await _departmentBusiness.GetDepartments(new DepartmentRequestModel());
                if (deptResult.Status)
                    Departments = new ObservableCollection<DepartmentResponseModel>(deptResult.ResponseData ?? new());

                if (teacherId > 0)
                {
                    IsEditMode = true;
                    FormTitle  = "Edit Teacher";
                    var result = await _teacherBusiness.GetTeacherById(new TeacherRequestModel { TeacherId = teacherId });
                    if (result.Status && result.ResponseData != null)
                    {
                        var t      = result.ResponseData;
                        TeacherId  = t.TeacherId;
                        Name       = t.Name;
                        Phone      = t.Phone;
                        DepartmentId = t.DepartmentId;
                    }
                }
                else
                {
                    IsEditMode = false;
                    FormTitle  = "Add Teacher";
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "TeacherFormViewModel.Initialize: Error");
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
                var request = new TeacherRequestModel
                {
                    TeacherId    = TeacherId,
                    Name         = Name,
                    Phone        = Phone,
                    DepartmentId = DepartmentId
                };

                // UpdateTeacher returns AppResult<bool>, InsertTeacher returns AppResult<int>
                // — handle separately to avoid ternary type mismatch
                bool success;
                string? message;

                if (IsEditMode)
                {
                    var updateResult = await _teacherBusiness.UpdateTeacher(request);
                    success = updateResult.Status;
                    message = updateResult.Message;
                }
                else
                {
                    var insertResult = await _teacherBusiness.InsertTeacher(request);
                    success = insertResult.Status;
                    message = insertResult.Message;
                }

                if (success)
                {
                    SetSuccess(IsEditMode ? "Teacher updated successfully." : "Teacher added successfully.");
                    SaveCompleted?.Invoke();
                }
                else
                    SetError(message ?? "Failed to save teacher.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "TeacherFormViewModel.Save: Error");
                SetError("Unexpected error saving teacher.");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
