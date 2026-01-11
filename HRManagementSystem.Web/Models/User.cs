using System.ComponentModel.DataAnnotations;

namespace HRManagementSystem.Web.Models
{
    public class User
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(80)]
        public string Username { get; set; }
        
        [Required]
        [EmailAddress]
        [StringLength(120)]
        public string Email { get; set; }
        
        [Required]
        public string PasswordHash { get; set; }
        
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }
        
        [Required]
        [StringLength(50)]
        public string LastName { get; set; }
        
        [Required]
        [StringLength(20)]
        public string EmployeeCode { get; set; }
        
        public int? DepartmentId { get; set; }
        public int? PositionId { get; set; }
        public int? ManagerId { get; set; }
        public DateTime? HireDate { get; set; }
        public string Phone { get; set; }
        public string Notes { get; set; }
        public string Status { get; set; } = "active";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? PasswordChangedAt { get; set; } = DateTime.UtcNow;
        public bool MustChangePassword { get; set; } = true;
        
        // Navigation properties
        public virtual Department? Department { get; set; }
        public virtual Position? Position { get; set; }
        public virtual User? Manager { get; set; }
        public virtual ICollection<UserRole>? UserRoles { get; set; }
        public virtual ICollection<ProjectAssignment>? ProjectAssignments { get; set; }
        public virtual ICollection<Attendance>? Attendances { get; set; }
        public virtual ICollection<LeaveRequest>? LeaveRequests { get; set; }
        public virtual ICollection<User>? Subordinates { get; set; }
        public virtual ICollection<Project>? ManagedProjects { get; set; }
        public virtual ICollection<Worklog>? Worklogs { get; set; }
        public virtual ICollection<Department>? ManagedDepartments { get; set; }
        
        public string FullName => $"{FirstName} {LastName}";
    }
}