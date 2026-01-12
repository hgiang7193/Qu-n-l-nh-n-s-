using Microsoft.EntityFrameworkCore;
using HRManagementSystem.Web.Models;

namespace HRManagementSystem.Web.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectAssignment> ProjectAssignments { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<LeaveRequest> LeaveRequests { get; set; }
        public DbSet<Worklog> Worklogs { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<Shift> Shifts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure UserRole relationship (many-to-many)
            modelBuilder.Entity<UserRole>()
                .HasKey(ur => new { ur.UserId, ur.RoleId });

            modelBuilder.Entity<UserRole>()
                .HasOne(u => u.User)
                .WithMany(ur => ur.UserRoles)
                .HasForeignKey(u => u.UserId);

            modelBuilder.Entity<UserRole>()
                .HasOne(r => r.Role)
                .WithMany(ur => ur.UserRoles)
                .HasForeignKey(r => r.RoleId);

            // Configure RolePermission relationship (many-to-many)
            modelBuilder.Entity<RolePermission>()
                .HasKey(rp => new { rp.RoleId, rp.PermissionId });

            modelBuilder.Entity<RolePermission>()
                .HasOne(r => r.Role)
                .WithMany(rp => rp.RolePermissions)
                .HasForeignKey(r => r.RoleId);

            modelBuilder.Entity<RolePermission>()
                .HasOne(p => p.Permission)
                .WithMany(rp => rp.RolePermissions)
                .HasForeignKey(p => p.PermissionId);

            // Configure ProjectAssignment relationship
            modelBuilder.Entity<ProjectAssignment>()
                .HasOne(pa => pa.Project)
                .WithMany(p => p.ProjectAssignments)
                .HasForeignKey(pa => pa.ProjectId);

            modelBuilder.Entity<ProjectAssignment>()
                .HasOne(pa => pa.Employee)
                .WithMany(u => u.ProjectAssignments)
                .HasForeignKey(pa => pa.EmployeeId);

            // Configure Attendance relationship
            modelBuilder.Entity<Attendance>()
                .HasOne(a => a.Employee)
                .WithMany(u => u.Attendances)
                .HasForeignKey(a => a.EmployeeId);

            // Make sure no relationship exists between Attendance and Shift
            // This prevents EF from creating a ShiftId foreign key column

            // Configure LeaveRequest relationship
            modelBuilder.Entity<LeaveRequest>()
                .HasOne(lr => lr.Employee)
                .WithMany(u => u.LeaveRequests)
                .HasForeignKey(lr => lr.EmployeeId);

            modelBuilder.Entity<LeaveRequest>()
                .HasOne(lr => lr.ApprovedByUser)
                .WithMany()
                .HasForeignKey(lr => lr.ApprovedBy);

            // Configure Worklog relationship
            modelBuilder.Entity<Worklog>()
                .HasOne(w => w.Employee)
                .WithMany(u => u.Worklogs)
                .HasForeignKey(w => w.EmployeeId);

            modelBuilder.Entity<Worklog>()
                .HasOne(w => w.Project)
                .WithMany(p => p.Worklogs)
                .HasForeignKey(w => w.ProjectId);

            // Configure Department relationship
            modelBuilder.Entity<Department>()
                .HasOne(d => d.Manager)
                .WithMany(u => u.ManagedDepartments!)
                .HasForeignKey(d => d.ManagerId);

            modelBuilder.Entity<Department>()
                .HasOne(d => d.Parent)
                .WithMany(d => d.Children)
                .HasForeignKey(d => d.ParentId);

            // Configure User relationships
            modelBuilder.Entity<User>()
                .HasOne(u => u.Manager)
                .WithMany(u => u.Subordinates)
                .HasForeignKey(u => u.ManagerId);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Department)
                .WithMany(d => d.Employees)
                .HasForeignKey(u => u.DepartmentId);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Position)
                .WithMany(p => p.Users)
                .HasForeignKey(u => u.PositionId);

            // Configure Project relationship
            modelBuilder.Entity<Project>()
                .HasOne(p => p.ProjectManager)
                .WithMany(u => u.ManagedProjects)
                .HasForeignKey(p => p.ProjectManagerId);

            // Configure Shift entity
            modelBuilder.Entity<Shift>()
                .Property(s => s.StartTime)
                .HasColumnType("time");
            
            modelBuilder.Entity<Shift>()
                .Property(s => s.EndTime)
                .HasColumnType("time");
            
            modelBuilder.Entity<Shift>()
                .Property(s => s.Status)
                .HasDefaultValue("Active");

            // Seed initial data
            SeedInitialData(modelBuilder);
        }

        private void SeedInitialData(ModelBuilder modelBuilder)
        {
            // Seed roles
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "admin", Description = "Quản trị viên với quyền truy cập đầy đủ hệ thống" },
                new Role { Id = 2, Name = "hr", Description = "Nhân sự với quyền hạn nhân sự cụ thể" },
                new Role { Id = 3, Name = "employee", Description = "Nhân viên thông thường với quyền truy cập hạn chế" },
                new Role { Id = 4, Name = "pm", Description = "Quản lý dự án với quyền hạn dự án cụ thể" }
            );

            // Seed permissions
            modelBuilder.Entity<Permission>().HasData(
                new Permission { Id = 1, Name = "can_view_employees", Description = "Có thể xem thông tin nhân viên" },
                new Permission { Id = 2, Name = "can_edit_employees", Description = "Có thể chỉnh sửa thông tin nhân viên" },
                new Permission { Id = 3, Name = "can_delete_employees", Description = "Có thể xóa nhân viên" },
                new Permission { Id = 4, Name = "can_view_projects", Description = "Có thể xem thông tin dự án" },
                new Permission { Id = 5, Name = "can_edit_projects", Description = "Có thể chỉnh sửa thông tin dự án" },
                new Permission { Id = 6, Name = "can_view_reports", Description = "Có thể xem báo cáo" },
                new Permission { Id = 7, Name = "can_manage_roles", Description = "Có thể quản lý vai trò và quyền hạn" }
            );

            // Seed departments
            modelBuilder.Entity<Department>().HasData(
                new Department { Id = 1, Name = "Human Resources", Code = "HR", Description = "Human Resources Department", Status = "active" },
                new Department { Id = 2, Name = "Information Technology", Code = "IT", Description = "Information Technology Department", Status = "active" },
                new Department { Id = 3, Name = "Finance", Code = "FIN", Description = "Finance Department", Status = "active" },
                new Department { Id = 4, Name = "Marketing", Code = "MKT", Description = "Marketing Department", Status = "active" },
                new Department { Id = 5, Name = "Sales", Code = "SALES", Description = "Sales Department", Status = "active" }
            );

            // Seed positions
            modelBuilder.Entity<Position>().HasData(
                new Position { Id = 1, Name = "CEO", Description = "Giám đốc điều hành", Status = "active" },
                new Position { Id = 2, Name = "CTO", Description = "Giám đốc công nghệ", Status = "active" },
                new Position { Id = 3, Name = "HR Manager", Description = "Quản lý nhân sự", Status = "active" },
                new Position { Id = 4, Name = "Team Lead", Description = "Trưởng nhóm/Supervisor", Status = "active" },
                new Position { Id = 5, Name = "Senior Developer", Description = "Lập trình viên cao cấp", Status = "active" },
                new Position { Id = 6, Name = "Developer", Description = "Lập trình viên", Status = "active" },
                new Position { Id = 7, Name = "Junior Developer", Description = "Lập trình viên thực tập", Status = "active" },
                new Position { Id = 8, Name = "HR Specialist", Description = "Chuyên viên nhân sự", Status = "active" },
                new Position { Id = 9, Name = "Accountant", Description = "Kế toán", Status = "active" },
                new Position { Id = 10, Name = "Marketing Specialist", Description = "Chuyên viên marketing", Status = "active" }
            );
        }
    }
}