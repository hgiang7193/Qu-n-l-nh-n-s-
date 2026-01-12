using AutoMapper;
using HRManagementSystem.Web.Models;
using HRManagementSystem.Web.Models.Dtos;

namespace HRManagementSystem.Web.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Source -> Target
            CreateMap<Role, RoleDto>();
            CreateMap<CreateRoleDto, Role>();
            CreateMap<UpdateRoleDto, Role>();

            CreateMap<Department, DepartmentDto>();
            CreateMap<CreateDepartmentDto, Department>();
            CreateMap<UpdateDepartmentDto, Department>();

            CreateMap<Position, PositionDto>();
            CreateMap<CreatePositionDto, Position>();
            CreateMap<UpdatePositionDto, Position>();

            CreateMap<User, EmployeeDto>();
            CreateMap<CreateEmployeeDto, User>();
            CreateMap<UpdateEmployeeDto, User>();

            // Project mappings
            CreateMap<Project, ProjectDto>();
            CreateMap<CreateProjectDto, Project>();
            CreateMap<UpdateProjectDto, Project>();

            // Shift mappings
            CreateMap<Shift, ShiftDto>();
            CreateMap<CreateShiftDto, Shift>();
            CreateMap<UpdateShiftDto, Shift>();

            // Attendance mappings
            CreateMap<Attendance, AttendanceDto>()
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.Employee != null ? (src.Employee.FirstName + " " + src.Employee.LastName) : ""));
            CreateMap<CreateAttendanceDto, Attendance>();
            CreateMap<UpdateAttendanceDto, Attendance>();

            // LeaveRequest mappings
            CreateMap<LeaveRequest, LeaveRequestDto>()
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.Employee != null ? (src.Employee.FirstName + " " + src.Employee.LastName) : ""))
                .ForMember(dest => dest.ApprovedByName, opt => opt.MapFrom(src => src.ApprovedByUser != null ? (src.ApprovedByUser.FirstName + " " + src.ApprovedByUser.LastName) : ""));
            CreateMap<CreateLeaveRequestDto, LeaveRequest>();
            CreateMap<UpdateLeaveRequestDto, LeaveRequest>();

            // Permission mappings
            CreateMap<Permission, PermissionDto>();
            CreateMap<CreatePermissionDto, Permission>();
            CreateMap<UpdatePermissionDto, Permission>();

            // ProjectAssignment mappings
            CreateMap<ProjectAssignment, ProjectAssignmentDto>()
                .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Project != null ? src.Project.Name : ""))
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.Employee != null ? (src.Employee.FirstName + " " + src.Employee.LastName) : ""));
            CreateMap<CreateProjectAssignmentDto, ProjectAssignment>();
            CreateMap<UpdateProjectAssignmentDto, ProjectAssignment>();

            // Worklog mappings
            CreateMap<Worklog, WorklogDto>()
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.Employee != null ? (src.Employee.FirstName + " " + src.Employee.LastName) : ""))
                .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Project != null ? src.Project.Name : ""));
            CreateMap<CreateWorklogDto, Worklog>();
            CreateMap<UpdateWorklogDto, Worklog>();

            // User mappings
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department != null ? src.Department.Name : ""))
                .ForMember(dest => dest.PositionName, opt => opt.MapFrom(src => src.Position != null ? src.Position.Name : ""))
                .ForMember(dest => dest.ManagerName, opt => opt.MapFrom(src => src.Manager != null ? (src.Manager.FirstName + " " + src.Manager.LastName) : ""));
            CreateMap<CreateUserDto, User>();
            CreateMap<UpdateUserDto, User>();

            // UserRole mappings
            CreateMap<UserRole, UserRoleDto>()
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User != null ? src.User.Username : ""))
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role != null ? src.Role.Name : ""));
            CreateMap<CreateUserRoleDto, UserRole>();

            // RolePermission mappings
            CreateMap<RolePermission, RolePermissionDto>()
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role != null ? src.Role.Name : ""))
                .ForMember(dest => dest.PermissionName, opt => opt.MapFrom(src => src.Permission != null ? src.Permission.Name : ""));
            CreateMap<CreateRolePermissionDto, RolePermission>();
        }
    }
}