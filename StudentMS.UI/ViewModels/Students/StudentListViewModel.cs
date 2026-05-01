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
        private int _filterDepartmentId;

        /// <summary>Raised when the user clicks Add or Edit — the View opens the form.</summary>
        public event Action<int>? OpenFormRequested; // 0 = add, >0 = edit

        [RelayCommand]
        private async Task LoadAsync()
        {
            IsBusy = true;
            ClearMessages();
            try
            {
                var deptResult = await _departmentBusiness.GetDepartments(new DepartmentRequestModel());
                if (deptResult.Status)
                    Departments = new ObservableCollection<DepartmentResponseModel>(deptResult.ResponseData ?? new());

                var request = new StudentRequestModel { DepartmentId = FilterDepartmentId };
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
