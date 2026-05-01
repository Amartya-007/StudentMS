using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Serilog;
using StudentMS.Business.Interfaces;
using StudentMS.Models.RequestModels;
using StudentMS.Models.ResponseModels;
using StudentMS.UI.ViewModels.Base;
using System.Collections.ObjectModel;

namespace StudentMS.UI.ViewModels.Students
{
    public partial class StudentFormViewModel : BaseViewModel
    {
        private readonly IStudentBusiness    _studentBusiness;
        private readonly IDepartmentBusiness _departmentBusiness;

        public StudentFormViewModel(IStudentBusiness studentBusiness, IDepartmentBusiness departmentBusiness)
        {
            _studentBusiness    = studentBusiness;
            _departmentBusiness = departmentBusiness;
        }

        // ── Form fields ──────────────────────────────────────────────────────────
        [ObservableProperty] private int       _studentId;
        [ObservableProperty] private string?   _name;
        [ObservableProperty] private DateTime? _dateOfBirth = DateTime.Today.AddYears(-18);
        [ObservableProperty] private string?   _gender;
        [ObservableProperty] private string?   _phone;
        [ObservableProperty] private string?   _address;
        [ObservableProperty] private int       _departmentId;
        [ObservableProperty] private string    _formTitle = "Add Student";
        [ObservableProperty] private bool      _isEditMode;

        [ObservableProperty]
        private ObservableCollection<DepartmentResponseModel> _departments = new();

        public IReadOnlyList<string> GenderOptions { get; } = new[] { "Male", "Female", "Other" };

        /// <summary>Raised when save completes — the View closes the dialog.</summary>
        public event Action? SaveCompleted;

        // ── Load departments + optionally load student for edit ──────────────────
        [RelayCommand]
        private async Task InitializeAsync(int studentId)
        {
            IsBusy = true;
            ClearMessages();
            try
            {
                var deptResult = await _departmentBusiness.GetDepartments(new DepartmentRequestModel());
                if (deptResult.Status)
                    Departments = new ObservableCollection<DepartmentResponseModel>(deptResult.ResponseData ?? new());

                if (studentId > 0)
                {
                    IsEditMode = true;
                    FormTitle  = "Edit Student";
                    var result = await _studentBusiness.GetStudentById(new StudentRequestModel { StudentId = studentId });
                    if (result.Status && result.ResponseData != null)
                    {
                        var s        = result.ResponseData;
                        StudentId    = s.StudentId;
                        Name         = s.Name;
                        DateOfBirth  = s.DOB ?? DateTime.Today.AddYears(-18);
                        Gender       = s.Gender;
                        Phone        = s.Phone;
                        Address      = s.Address;
                        DepartmentId = s.DepartmentId;
                    }
                }
                else
                {
                    IsEditMode = false;
                    FormTitle  = "Add Student";
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "StudentFormViewModel.Initialize: Error");
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
                var request = new StudentRequestModel
                {
                    StudentId    = StudentId,
                    Name         = Name,
                    DOB          = DateOfBirth,
                    Gender       = Gender,
                    Phone        = Phone,
                    Address      = Address,
                    DepartmentId = DepartmentId
                };

                // UpdateStudent returns AppResult<bool>, InsertStudent returns AppResult<int>
                // — handle separately to avoid ternary type mismatch
                bool success;
                string? message;

                if (IsEditMode)
                {
                    var updateResult = await _studentBusiness.UpdateStudent(request);
                    success = updateResult.Status;
                    message = updateResult.Message;
                }
                else
                {
                    var insertResult = await _studentBusiness.InsertStudent(request);
                    success = insertResult.Status;
                    message = insertResult.Message;
                }

                if (success)
                {
                    SetSuccess(IsEditMode ? "Student updated successfully." : "Student added successfully.");
                    SaveCompleted?.Invoke();
                }
                else
                {
                    SetError(message ?? "Failed to save student.");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "StudentFormViewModel.Save: Error");
                SetError("Unexpected error saving student.");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
