using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Serilog;
using StudentMS.Business.Interfaces;
using StudentMS.Common.Session;
using StudentMS.Models.RequestModels;
using StudentMS.Models.ResponseModels;
using StudentMS.UI.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Windows;

namespace StudentMS.UI.ViewModels.Students
{
    public partial class StudentListViewModel : BaseViewModel
    {
        private readonly IStudentBusiness _studentBusiness;
        private readonly IDepartmentBusiness _departmentBusiness;

        public StudentListViewModel(IStudentBusiness studentBusiness, IDepartmentBusiness departmentBusiness)
        {
            _studentBusiness    = studentBusiness;
            _departmentBusiness = departmentBusiness;
        }

        [ObservableProperty]
        private ObservableCollection<StudentResponseModel> _students = new();

        [ObservableProperty]
        private ObservableCollection<DepartmentResponseModel> _departments = new();

        [ObservableProperty]
        private StudentResponseModel? _selectedStudent;

        [ObservableProperty]
        private string? _searchText;

        [ObservableProperty]
        private int? _filterDepartmentId;  // nullable so ComboBox can be cleared to null = "All"

        // ── Req 3.4: Read-only mode for Teacher role ─────────────────────────

        /// <summary>True when the current user is a Teacher (read-only access).</summary>
        public bool IsReadOnly => AppSession.Current.IsTeacher;

        /// <summary>Hides add/edit/delete controls for Teacher role.</summary>
        public Visibility ActionVisibility => IsReadOnly ? Visibility.Collapsed : Visibility.Visible;

        /// <summary>Raised when the user clicks Add or Edit — the View opens the form.</summary>
        public event Action<int>? OpenFormRequested; // 0 = add, >0 = edit

        [RelayCommand]
        private async Task LoadAsync()
        {
            IsBusy = true;
            ClearMessages();
            try
            {
                // Only load departments on first load (when collection is empty)
                if (!Departments.Any())
                {
                    var deptResult = await _departmentBusiness.GetDepartments(new DepartmentRequestModel());
                    if (deptResult.Status)
                        Departments = new ObservableCollection<DepartmentResponseModel>(deptResult.ResponseData ?? new());
                }

                var request = new StudentRequestModel { DepartmentId = FilterDepartmentId ?? 0 };
                var result  = await _studentBusiness.GetStudents(request);

                if (result.Status)
                    Students = new ObservableCollection<StudentResponseModel>(result.ResponseData ?? new());
                else
                    SetError(result.Message ?? "Failed to load students.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "StudentListViewModel.Load: Error");
                SetError("Unexpected error loading students.");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private void AddStudent() => OpenFormRequested?.Invoke(0);

        [RelayCommand]
        private async Task ClearFilterAsync()
        {
            FilterDepartmentId = null;
            await LoadAsync();
        }

        [RelayCommand]
        private void EditStudent(StudentResponseModel student)
        {
            SelectedStudent = student;
            OpenFormRequested?.Invoke(student.StudentId);
        }

        [RelayCommand]
        private async Task DeleteStudentAsync(int studentId)
        {
            IsBusy = true;
            ClearMessages();
            try
            {
                var result = await _studentBusiness.DeleteStudent(new StudentRequestModel { StudentId = studentId });
                if (result.Status)
                {
                    SetSuccess("Student deleted successfully.");
                    await LoadAsync();
                }
                else
                {
                    SetError(result.Message ?? "Failed to delete student.");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "StudentListViewModel.Delete: Error");
                SetError("Unexpected error deleting student.");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
