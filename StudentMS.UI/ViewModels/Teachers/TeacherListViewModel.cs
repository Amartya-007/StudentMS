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
    public partial class TeacherListViewModel : BaseViewModel
    {
        private readonly ITeacherBusiness    _teacherBusiness;
        private readonly IDepartmentBusiness _departmentBusiness;

        public TeacherListViewModel(ITeacherBusiness teacherBusiness, IDepartmentBusiness departmentBusiness)
        {
            _teacherBusiness    = teacherBusiness;
            _departmentBusiness = departmentBusiness;
        }

        [ObservableProperty] private ObservableCollection<TeacherResponseModel>    _teachers    = new();
        [ObservableProperty] private ObservableCollection<DepartmentResponseModel> _departments = new();
        [ObservableProperty] private TeacherResponseModel? _selectedTeacher;
        [ObservableProperty] private int _filterDepartmentId;

        public event Action<int>? OpenFormRequested;

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

                var result = await _teacherBusiness.GetTeachers(new TeacherRequestModel { DepartmentId = FilterDepartmentId });
                if (result.Status)
                    Teachers = new ObservableCollection<TeacherResponseModel>(result.ResponseData ?? new());
                else
                    SetError(result.Message ?? "Failed to load teachers.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "TeacherListViewModel.Load: Error");
                SetError("Unexpected error loading teachers.");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand] private void AddTeacher() => OpenFormRequested?.Invoke(0);

        [RelayCommand]
        private void EditTeacher(TeacherResponseModel teacher)
        {
            SelectedTeacher = teacher;
            OpenFormRequested?.Invoke(teacher.TeacherId);
        }

        [RelayCommand]
        private async Task DeleteTeacherAsync(int teacherId)
        {
            IsBusy = true;
            ClearMessages();
            try
            {
                var result = await _teacherBusiness.DeleteTeacher(new TeacherRequestModel { TeacherId = teacherId });
                if (result.Status)
                {
                    SetSuccess("Teacher deleted successfully.");
                    await LoadAsync();
                }
                else
                    SetError(result.Message ?? "Failed to delete teacher.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "TeacherListViewModel.Delete: Error");
                SetError("Unexpected error deleting teacher.");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
