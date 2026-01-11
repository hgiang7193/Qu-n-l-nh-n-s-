using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using HRManagementSystem.Web.Data;
using HRManagementSystem.Web.Models;
using BCrypt.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace HRManagementSystem.Web.Services
{
    public class DatabaseInitializer
    {
        private readonly ApplicationDbContext _context;

        public DatabaseInitializer(ApplicationDbContext context)
        {
            _context = context;
        }

        private async Task<bool> CheckIfTableExists(string tableName)
        {
            try
            {
                var connection = _context.Database.GetDbConnection();
                if (connection.State != System.Data.ConnectionState.Open)
                {
                    await connection.OpenAsync();
                }

                using var command = connection.CreateCommand();
                command.CommandText = "SELECT COUNT(*) FROM information_schema.tables WHERE table_schema = DATABASE() AND table_name = @tableName;";
                var parameter = command.CreateParameter();
                parameter.ParameterName = "@tableName";
                parameter.Value = tableName;
                command.Parameters.Add(parameter);

                var result = await command.ExecuteScalarAsync();
                return Convert.ToInt32(result) > 0;
            }
            catch
            {
                return false;
            }
        }

        private async Task EnsureTablesExist()
        {
            try
            {
                // Create tables according to the model if they don't exist
                _context.Database.EnsureCreated();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating database tables: {ex.Message}");
                throw;
            }
        }

        public async Task InitializeAsync()
        {
            // Ensure all tables are created according to the model
            // Ensure all required tables exist, including UserRoles
            await EnsureTablesExist();

            // Execute the SQL file to initialize the database with data
            await ExecuteSqlScriptFromFile();
        }

        private async Task ExecuteSqlScriptFromFile()
        {
            try
            {
                // Look for the SQL file in the correct location
                var baseDir = Directory.GetCurrentDirectory();
                Console.WriteLine($"Current directory: {baseDir}");
                
                // Try different possible paths
                string[] possiblePaths = {
                    Path.Combine(baseDir, "..", "..", "Database", "hr_system_complete_db.sql"),
                    Path.Combine(baseDir, "..", "Database", "hr_system_complete_db.sql"),
                    Path.Combine(baseDir, "Database", "hr_system_complete_db.sql"),
                    Path.Combine(baseDir, "..", "..", "..", "Database", "hr_system_complete_db.sql"),
                    "..\\..\\Database\\hr_system_complete_db.sql",
                    "..\\Database\\hr_system_complete_db.sql",
                    ".\\Database\\hr_system_complete_db.sql",
                    "Database\\hr_system_complete_db.sql"
                };
                
                string sqlFilePath = null;
                foreach (var path in possiblePaths)
                {
                    Console.WriteLine($"Checking path: {path}");
                    if (File.Exists(path))
                    {
                        sqlFilePath = path;
                        Console.WriteLine($"Found SQL file at: {path}");
                        break;
                    }
                    
                    // Also try with full path resolution
                    var fullPath = Path.GetFullPath(path);
                    Console.WriteLine($"Checking full path: {fullPath}");
                    if (File.Exists(fullPath))
                    {
                        sqlFilePath = fullPath;
                        Console.WriteLine($"Found SQL file at: {fullPath}");
                        break;
                    }
                }
                
                if (sqlFilePath != null && File.Exists(sqlFilePath))
                {
                    // Instead of executing raw SQL which has schema mismatches, 
                    // let's populate the database with the intended data programmatically
                    await PopulateDatabaseWithSampleData();
                }
                else
                {
                    Console.WriteLine($"SQL file not found in any expected location. Available files in Database folder:");
                    var dbDir = Path.Combine(baseDir, "..", "..", "Database");
                    if (Directory.Exists(dbDir))
                    {
                        foreach (var file in Directory.GetFiles(dbDir, "*.sql"))
                        {
                            Console.WriteLine($"  - {Path.GetFileName(file)}");
                        }
                    }
                    
                    // Fallback: Create basic users if SQL file is not found
                    await CreateBasicUsers();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error executing SQL script: {ex.Message}");
                
                // Fallback: Create basic users if SQL execution fails
                await CreateBasicUsers();
            }
        }
        
        private async Task PopulateDatabaseWithSampleData()
        {
            Console.WriteLine("Populating database with sample data from SQL file specification...");
            
            // Clear existing data
            try
            {
                await _context.Database.ExecuteSqlRawAsync("SET FOREIGN_KEY_CHECKS = 0;");
                await _context.Database.ExecuteSqlRawAsync("DELETE FROM UserRoles;");
                await _context.Database.ExecuteSqlRawAsync("DELETE FROM ProjectAssignments;");
                await _context.Database.ExecuteSqlRawAsync("DELETE FROM Attendances;");
                await _context.Database.ExecuteSqlRawAsync("DELETE FROM LeaveRequests;");
                await _context.Database.ExecuteSqlRawAsync("DELETE FROM Worklogs;");
                await _context.Database.ExecuteSqlRawAsync("DELETE FROM Departments;");
                await _context.Database.ExecuteSqlRawAsync("DELETE FROM Projects;");
                await _context.Database.ExecuteSqlRawAsync("DELETE FROM Users;");
                await _context.Database.ExecuteSqlRawAsync("DELETE FROM Roles;");
                await _context.Database.ExecuteSqlRawAsync("DELETE FROM Permissions;");
                await _context.Database.ExecuteSqlRawAsync("DELETE FROM RolePermission;");
                await _context.Database.ExecuteSqlRawAsync("DELETE FROM Shifts;");
                await _context.Database.ExecuteSqlRawAsync("DELETE FROM Positions;");
                await _context.Database.ExecuteSqlRawAsync("SET FOREIGN_KEY_CHECKS = 1;");
            }
            catch (Exception deleteEx)
            {
                Console.WriteLine($"Error clearing tables: {deleteEx.Message}");
            }
            
            // Create departments
            var departments = new List<Department>
            {
                new Department { Name = "Phòng Nhân Sự", Code = "HR", Description = "Phòng Nhân Sự", Status = "active", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                new Department { Name = "Phòng Công Nghệ Thông Tin", Code = "IT", Description = "Phòng Công Nghệ Thông Tin", Status = "active", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                new Department { Name = "Phòng Tài Chính", Code = "FIN", Description = "Phòng Tài Chính", Status = "active", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                new Department { Name = "Phòng Marketing", Code = "MKT", Description = "Phòng Marketing", Status = "active", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                new Department { Name = "Phòng Kinh Doanh", Code = "SALES", Description = "Phòng Kinh Doanh", Status = "active", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now }
            };
            
            _context.Departments.AddRange(departments);
            await _context.SaveChangesAsync();
            
            // Create positions
            var positions = new List<Position>
            {
                new Position { Name = "Giám đốc điều hành", Description = "Giám đốc điều hành", Status = "active", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                new Position { Name = "Giám đốc công nghệ", Description = "Giám đốc công nghệ", Status = "active", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                new Position { Name = "Quản lý nhân sự", Description = "Quản lý nhân sự", Status = "active", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                new Position { Name = "Trưởng nhóm/Supervisor", Description = "Trưởng nhóm/Supervisor", Status = "active", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                new Position { Name = "Lập trình viên cao cấp", Description = "Lập trình viên cao cấp", Status = "active", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                new Position { Name = "Lập trình viên", Description = "Lập trình viên", Status = "active", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                new Position { Name = "Lập trình viên thực tập", Description = "Lập trình viên thực tập", Status = "active", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                new Position { Name = "Chuyên viên nhân sự", Description = "Chuyên viên nhân sự", Status = "active", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                new Position { Name = "Kế toán", Description = "Kế toán", Status = "active", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                new Position { Name = "Chuyên viên marketing", Description = "Chuyên viên marketing", Status = "active", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now }
            };
            
            _context.Positions.AddRange(positions);
            await _context.SaveChangesAsync();
            
            // Create shifts
            var shifts = new List<Shift>
            {
                new Shift { Name = "Ca Sáng", Description = "Ca làm việc buổi sáng", StartTime = TimeSpan.Parse("08:00:00"), EndTime = TimeSpan.Parse("12:00:00"), CreatedAt = DateTime.Now },
                new Shift { Name = "Ca Chiều", Description = "Ca làm việc buổi chiều", StartTime = TimeSpan.Parse("13:00:00"), EndTime = TimeSpan.Parse("17:00:00"), CreatedAt = DateTime.Now },
                new Shift { Name = "Ca Tối", Description = "Ca làm việc buổi tối", StartTime = TimeSpan.Parse("18:00:00"), EndTime = TimeSpan.Parse("22:00:00"), CreatedAt = DateTime.Now },
                new Shift { Name = "Ca Đêm", Description = "Ca làm việc ban đêm", StartTime = TimeSpan.Parse("22:00:00"), EndTime = TimeSpan.Parse("06:00:00"), CreatedAt = DateTime.Now }
            };
            
            _context.Shifts.AddRange(shifts);
            await _context.SaveChangesAsync();
            
            // Create projects
            var projects = new List<Project>
            {
                new Project { Name = "Dự Án Website", Code = "WEB001", Description = "Dự án phát triển website cho khách hàng", StartDate = DateTime.Now.AddDays(-10), EndDate = DateTime.Now.AddDays(150), Status = "active", ProjectType = "software", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                new Project { Name = "Dự Án Mobile App", Code = "MOB001", Description = "Dự án phát triển ứng dụng di động", StartDate = DateTime.Now.AddDays(-5), EndDate = DateTime.Now.AddDays(200), Status = "active", ProjectType = "software", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                new Project { Name = "Dự Án ERP", Code = "ERP001", Description = "Dự án hệ thống quản lý doanh nghiệp", StartDate = DateTime.Now.AddDays(-30), EndDate = DateTime.Now.AddDays(300), Status = "active", ProjectType = "software", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now }
            };
            
            _context.Projects.AddRange(projects);
            await _context.SaveChangesAsync();
            
            // Create roles
            var roles = new List<Role>
            {
                new Role { Name = "admin", Description = "Quản trị viên với quyền truy cập đầy đủ hệ thống", CreatedAt = DateTime.Now },
                new Role { Name = "hr", Description = "Nhân sự với quyền hạn nhân sự cụ thể", CreatedAt = DateTime.Now },
                new Role { Name = "employee", Description = "Nhân viên thông thường với quyền truy cập hạn chế", CreatedAt = DateTime.Now },
                new Role { Name = "pm", Description = "Quản lý dự án với quyền hạn dự án cụ thể", CreatedAt = DateTime.Now }
            };
            
            _context.Roles.AddRange(roles);
            await _context.SaveChangesAsync();
            
            // Create permissions
            var permissions = new List<Permission>
            {
                new Permission { Name = "can_view_employees", Description = "Có thể xem thông tin nhân viên", CreatedAt = DateTime.Now },
                new Permission { Name = "can_edit_employees", Description = "Có thể chỉnh sửa thông tin nhân viên", CreatedAt = DateTime.Now },
                new Permission { Name = "can_delete_employees", Description = "Có thể xóa nhân viên", CreatedAt = DateTime.Now },
                new Permission { Name = "can_view_projects", Description = "Có thể xem thông tin dự án", CreatedAt = DateTime.Now },
                new Permission { Name = "can_edit_projects", Description = "Có thể chỉnh sửa thông tin dự án", CreatedAt = DateTime.Now },
                new Permission { Name = "can_view_reports", Description = "Có thể xem báo cáo", CreatedAt = DateTime.Now },
                new Permission { Name = "can_manage_roles", Description = "Có thể quản lý vai trò và quyền hạn", CreatedAt = DateTime.Now }
            };
            
            _context.Permissions.AddRange(permissions);
            await _context.SaveChangesAsync();
            
            // Create users with the exact data from your SQL file
            var users = new List<User>
            {
                new User { Username = "admin", Email = "admin@company.com", FirstName = "Admin", LastName = "System", EmployeeCode = "EMP001", Phone = "0123456789", HireDate = DateTime.Parse("2025-01-01"), Status = "active", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, PasswordChangedAt = DateTime.Now, MustChangePassword = false, Notes = "System Administrator" },
                new User { Username = "john.smith", Email = "john@company.com", FirstName = "John", LastName = "Smith", EmployeeCode = "EMP002", Phone = "0987654321", HireDate = DateTime.Parse("2025-02-01"), Status = "active", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, PasswordChangedAt = DateTime.Now, MustChangePassword = false, Notes = "Sample Employee" },
                new User { Username = "jane.doe", Email = "jane@company.com", FirstName = "Jane", LastName = "Doe", EmployeeCode = "EMP003", Phone = "0912345678", HireDate = DateTime.Parse("2025-03-01"), Status = "active", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, PasswordChangedAt = DateTime.Now, MustChangePassword = false, Notes = "Sample Employee" },
                new User { Username = "mike.johnson", Email = "mike@company.com", FirstName = "Mike", LastName = "Johnson", EmployeeCode = "EMP004", Phone = "0923456789", HireDate = DateTime.Parse("2025-04-01"), Status = "active", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, PasswordChangedAt = DateTime.Now, MustChangePassword = false, Notes = "Sample Employee" },
                new User { Username = "sarah.williams", Email = "sarah@company.com", FirstName = "Sarah", LastName = "Williams", EmployeeCode = "EMP005", Phone = "0934567890", HireDate = DateTime.Parse("2025-05-01"), Status = "active", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, PasswordChangedAt = DateTime.Now, MustChangePassword = false, Notes = "Sample Employee" },
                new User { Username = "david.brown", Email = "david@company.com", FirstName = "David", LastName = "Brown", EmployeeCode = "EMP006", Phone = "0945678901", HireDate = DateTime.Parse("2025-01-15"), Status = "active", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, PasswordChangedAt = DateTime.Now, MustChangePassword = false, Notes = "Sample Employee" },
                new User { Username = "lisa.jones", Email = "lisa@company.com", FirstName = "Lisa", LastName = "Jones", EmployeeCode = "EMP007", Phone = "0956789012", HireDate = DateTime.Parse("2025-02-15"), Status = "active", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, PasswordChangedAt = DateTime.Now, MustChangePassword = false, Notes = "Sample Employee" },
                new User { Username = "tom.wilson", Email = "tom@company.com", FirstName = "Tom", LastName = "Wilson", EmployeeCode = "EMP008", Phone = "0967890123", HireDate = DateTime.Parse("2025-03-15"), Status = "active", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, PasswordChangedAt = DateTime.Now, MustChangePassword = false, Notes = "Sample Employee" },
                new User { Username = "amy.taylor", Email = "amy@company.com", FirstName = "Amy", LastName = "Taylor", EmployeeCode = "EMP009", Phone = "0978901234", HireDate = DateTime.Parse("2025-04-15"), Status = "active", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, PasswordChangedAt = DateTime.Now, MustChangePassword = false, Notes = "Sample Employee" },
                new User { Username = "chris.martinez", Email = "chris@company.com", FirstName = "Chris", LastName = "Martinez", EmployeeCode = "EMP010", Phone = "0989012345", HireDate = DateTime.Parse("2025-05-15"), Status = "active", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, PasswordChangedAt = DateTime.Now, MustChangePassword = false, Notes = "Sample Employee" },
                new User { Username = "jessica.davis", Email = "jessica@company.com", FirstName = "Jessica", LastName = "Davis", EmployeeCode = "EMP011", Phone = "0990123456", HireDate = DateTime.Parse("2025-06-01"), Status = "active", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, PasswordChangedAt = DateTime.Now, MustChangePassword = false, Notes = "Sample Employee" },
                new User { Username = "kevin.garcia", Email = "kevin@company.com", FirstName = "Kevin", LastName = "Garcia", EmployeeCode = "EMP012", Phone = "0901234567", HireDate = DateTime.Parse("2025-06-15"), Status = "active", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, PasswordChangedAt = DateTime.Now, MustChangePassword = false, Notes = "Sample Employee" }
            };
            
            // Set password hashes for all users
            foreach (var user in users)
            {
                if (user.Username == "admin")
                    user.PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123");
                else
                    user.PasswordHash = BCrypt.Net.BCrypt.HashPassword("Employee@123");
            }
            
            // Declare variables
            User adminUser = null, johnUser = null, janeUser = null, mikeUser = null, sarahUser = null, davidUser = null, lisaUser = null, tomUser = null, amyUser = null, chrisUser = null, jessicaUser = null, kevinUser = null;
            
            // Save users to generate IDs before assigning foreign keys
            _context.Users.AddRange(users);
            await _context.SaveChangesAsync();
            
            // Refresh the users to get their IDs and update with the department and position IDs
            await _context.SaveChangesAsync(); // First save to get IDs
            
            var allUsers = await _context.Users.ToListAsync();
            var allDepartments = await _context.Departments.ToListAsync();
            var allPositions = await _context.Positions.ToListAsync();
            
            // Now assign the users
            adminUser = allUsers.FirstOrDefault(u => u.Username == "admin");
            johnUser = allUsers.FirstOrDefault(u => u.Username == "john.smith");
            janeUser = allUsers.FirstOrDefault(u => u.Username == "jane.doe");
            mikeUser = allUsers.FirstOrDefault(u => u.Username == "mike.johnson");
            sarahUser = allUsers.FirstOrDefault(u => u.Username == "sarah.williams");
            davidUser = allUsers.FirstOrDefault(u => u.Username == "david.brown");
            lisaUser = allUsers.FirstOrDefault(u => u.Username == "lisa.jones");
            tomUser = allUsers.FirstOrDefault(u => u.Username == "tom.wilson");
            amyUser = allUsers.FirstOrDefault(u => u.Username == "amy.taylor");
            chrisUser = allUsers.FirstOrDefault(u => u.Username == "chris.martinez");
            jessicaUser = allUsers.FirstOrDefault(u => u.Username == "jessica.davis");
            kevinUser = allUsers.FirstOrDefault(u => u.Username == "kevin.garcia");
            
            // Map users to appropriate departments and positions
            if(adminUser != null) {
                var dept1 = allDepartments.FirstOrDefault(d => d.Code == "HR"); // HR
                var pos1 = allPositions.FirstOrDefault(p => p.Name == "Giám đốc điều hành"); // CEO
                if(dept1 != null && pos1 != null) {
                    adminUser.DepartmentId = dept1.Id;
                    adminUser.PositionId = pos1.Id;
                }
            }
            
            if(johnUser != null) {
                var dept2 = allDepartments.FirstOrDefault(d => d.Code == "IT"); // IT
                var pos5 = allPositions.FirstOrDefault(p => p.Name == "Lập trình viên cao cấp"); // Senior Developer
                if(dept2 != null && pos5 != null) {
                    johnUser.DepartmentId = dept2.Id;
                    johnUser.PositionId = pos5.Id;
                }
            }
            
            if(janeUser != null) {
                var dept3 = allDepartments.FirstOrDefault(d => d.Code == "FIN"); // Finance
                var pos9 = allPositions.FirstOrDefault(p => p.Name == "Kế toán"); // Accountant
                if(dept3 != null && pos9 != null) {
                    janeUser.DepartmentId = dept3.Id;
                    janeUser.PositionId = pos9.Id;
                }
            }
            
            if(mikeUser != null) {
                var dept2 = allDepartments.FirstOrDefault(d => d.Code == "IT"); // IT
                var pos6 = allPositions.FirstOrDefault(p => p.Name == "Lập trình viên"); // Developer
                if(dept2 != null && pos6 != null) {
                    mikeUser.DepartmentId = dept2.Id;
                    mikeUser.PositionId = pos6.Id;
                }
            }
            
            if(sarahUser != null) {
                var dept4 = allDepartments.FirstOrDefault(d => d.Code == "MKT"); // Marketing
                var pos10 = allPositions.FirstOrDefault(p => p.Name == "Chuyên viên marketing"); // Marketing Specialist
                if(dept4 != null && pos10 != null) {
                    sarahUser.DepartmentId = dept4.Id;
                    sarahUser.PositionId = pos10.Id;
                }
            }
            
            if(davidUser != null) {
                var dept2 = allDepartments.FirstOrDefault(d => d.Code == "IT"); // IT
                var pos5 = allPositions.FirstOrDefault(p => p.Name == "Lập trình viên cao cấp"); // Senior Developer
                if(dept2 != null && pos5 != null) {
                    davidUser.DepartmentId = dept2.Id;
                    davidUser.PositionId = pos5.Id;
                }
            }
            
            if(lisaUser != null) {
                var dept1 = allDepartments.FirstOrDefault(d => d.Code == "HR"); // HR
                var pos8 = allPositions.FirstOrDefault(p => p.Name == "Chuyên viên nhân sự"); // HR Specialist
                if(dept1 != null && pos8 != null) {
                    lisaUser.DepartmentId = dept1.Id;
                    lisaUser.PositionId = pos8.Id;
                }
            }
            
            if(tomUser != null) {
                var dept5 = allDepartments.FirstOrDefault(d => d.Code == "SALES"); // Sales
                var pos4 = allPositions.FirstOrDefault(p => p.Name == "Trưởng nhóm/Supervisor"); // Supervisor
                if(dept5 != null && pos4 != null) {
                    tomUser.DepartmentId = dept5.Id;
                    tomUser.PositionId = pos4.Id;
                }
            }
            
            if(amyUser != null) {
                var dept2 = allDepartments.FirstOrDefault(d => d.Code == "IT"); // IT
                var pos6 = allPositions.FirstOrDefault(p => p.Name == "Lập trình viên"); // Developer
                if(dept2 != null && pos6 != null) {
                    amyUser.DepartmentId = dept2.Id;
                    amyUser.PositionId = pos6.Id;
                }
            }
            
            if(chrisUser != null) {
                var dept3 = allDepartments.FirstOrDefault(d => d.Code == "FIN"); // Finance
                var pos9 = allPositions.FirstOrDefault(p => p.Name == "Kế toán"); // Accountant
                if(dept3 != null && pos9 != null) {
                    chrisUser.DepartmentId = dept3.Id;
                    chrisUser.PositionId = pos9.Id;
                }
            }
            
            if(jessicaUser != null) {
                var dept4 = allDepartments.FirstOrDefault(d => d.Code == "MKT"); // Marketing
                var pos10 = allPositions.FirstOrDefault(p => p.Name == "Chuyên viên marketing"); // Marketing Specialist
                if(dept4 != null && pos10 != null) {
                    jessicaUser.DepartmentId = dept4.Id;
                    jessicaUser.PositionId = pos10.Id;
                }
            }
            
            if(kevinUser != null) {
                var dept2 = allDepartments.FirstOrDefault(d => d.Code == "IT"); // IT
                var pos7 = allPositions.FirstOrDefault(p => p.Name == "Lập trình viên thực tập"); // Intern
                if(dept2 != null && pos7 != null) {
                    kevinUser.DepartmentId = dept2.Id;
                    kevinUser.PositionId = pos7.Id;
                }
            }
            
            await _context.SaveChangesAsync();
            
            // Assign roles to users
            var adminRole = roles.First(r => r.Name == "admin");
            var employeeRole = roles.First(r => r.Name == "employee");
            
            var userRoles = new List<UserRole>
            {
                new UserRole { UserId = adminUser.Id, RoleId = adminRole.Id },
                new UserRole { UserId = johnUser.Id, RoleId = employeeRole.Id },
                new UserRole { UserId = janeUser.Id, RoleId = employeeRole.Id },
                new UserRole { UserId = mikeUser.Id, RoleId = employeeRole.Id },
                new UserRole { UserId = sarahUser.Id, RoleId = employeeRole.Id }
            };
            
            _context.UserRoles.AddRange(userRoles);
            await _context.SaveChangesAsync();
            
            // Create some sample project assignments
            var projectAssignments = new List<ProjectAssignment>
            {
                new ProjectAssignment { ProjectId = projects[0].Id, EmployeeId = johnUser.Id, Role = "Developer", StartDate = DateTime.Now, EndDate = DateTime.Now.AddMonths(6), Status = "active", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                new ProjectAssignment { ProjectId = projects[0].Id, EmployeeId = mikeUser.Id, Role = "Tester", StartDate = DateTime.Now, EndDate = DateTime.Now.AddMonths(6), Status = "active", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                new ProjectAssignment { ProjectId = projects[1].Id, EmployeeId = johnUser.Id, Role = "Lead Developer", StartDate = DateTime.Now.AddDays(30), EndDate = DateTime.Now.AddMonths(8), Status = "active", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                new ProjectAssignment { ProjectId = projects[1].Id, EmployeeId = sarahUser.Id, Role = "Designer", StartDate = DateTime.Now.AddDays(30), EndDate = DateTime.Now.AddMonths(8), Status = "active", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now }
            };
            
            _context.ProjectAssignments.AddRange(projectAssignments);
            await _context.SaveChangesAsync();
            
            // Create some sample attendances
            var attendances = new List<Attendance>
            {
                new Attendance { EmployeeId = adminUser.Id, Date = DateTime.Today.AddDays(-1), CheckIn = DateTime.Parse(DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd") + " 08:15:00"), CheckOut = DateTime.Parse(DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd") + " 17:30:00"), Status = "late", CreatedAt = DateTime.Now },
                new Attendance { EmployeeId = adminUser.Id, Date = DateTime.Today, CheckIn = DateTime.Parse(DateTime.Today.ToString("yyyy-MM-dd") + " 08:00:00"), CheckOut = DateTime.Parse(DateTime.Today.ToString("yyyy-MM-dd") + " 17:00:00"), Status = "on_time", CreatedAt = DateTime.Now },
                new Attendance { EmployeeId = johnUser.Id, Date = DateTime.Today.AddDays(-1), CheckIn = DateTime.Parse(DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd") + " 08:20:00"), CheckOut = DateTime.Parse(DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd") + " 17:10:00"), Status = "late", CreatedAt = DateTime.Now },
                new Attendance { EmployeeId = johnUser.Id, Date = DateTime.Today, CheckIn = DateTime.Parse(DateTime.Today.ToString("yyyy-MM-dd") + " 08:05:00"), CheckOut = DateTime.Parse(DateTime.Today.ToString("yyyy-MM-dd") + " 17:05:00"), Status = "on_time", CreatedAt = DateTime.Now }
            };
            
            _context.Attendances.AddRange(attendances);
            await _context.SaveChangesAsync();
            
            // Create some sample worklogs
            var worklogs = new List<Worklog>
            {
                new Worklog { EmployeeId = johnUser.Id, ProjectId = projects[0].Id, Date = DateTime.Today.AddDays(-1), Hours = 8.0, Description = "Hoàn thành module đăng nhập", Status = "approved", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                new Worklog { EmployeeId = johnUser.Id, ProjectId = projects[0].Id, Date = DateTime.Today, Hours = 7.5, Description = "Sửa lỗi bảo mật", Status = "submitted", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                new Worklog { EmployeeId = mikeUser.Id, ProjectId = projects[0].Id, Date = DateTime.Today.AddDays(-1), Hours = 8.0, Description = "Test module đăng nhập", Status = "approved", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                new Worklog { EmployeeId = mikeUser.Id, ProjectId = projects[1].Id, Date = DateTime.Today, Hours = 6.0, Description = "Thiết kế giao diện chính", Status = "submitted", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now }
            };
            
            _context.Worklogs.AddRange(worklogs);
            await _context.SaveChangesAsync();
            
            // Create some sample leave requests
            var leaveRequests = new List<LeaveRequest>
            {
                new LeaveRequest { EmployeeId = johnUser.Id, LeaveType = "annual_leave", StartDate = DateTime.Now.AddMonths(1), EndDate = DateTime.Now.AddMonths(1).AddDays(5), Reason = "Nghỉ Tết dương lịch", Status = "approved", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                new LeaveRequest { EmployeeId = janeUser.Id, LeaveType = "sick_leave", StartDate = DateTime.Now.AddDays(1), EndDate = DateTime.Now.AddDays(1), Reason = "Ốm đau", Status = "pending", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now }
            };
            
            _context.LeaveRequests.AddRange(leaveRequests);
            await _context.SaveChangesAsync();
            
            Console.WriteLine("Database populated with sample data successfully!");
        }

        private async Task CreateBasicUsers()
        {
            // Check if admin user already exists
            var adminExists = await _context.Users.AnyAsync(u => u.Username == "admin");
            
            if (!adminExists)
            {
                // Create admin user
                var adminUser = new User
                {
                    Username = "admin",
                    Email = "admin@company.com",
                    FirstName = "System",
                    LastName = "Administrator",
                    EmployeeCode = "EMP001",
                    Phone = "+1234567890",
                    Notes = "System Administrator",
                    Status = "active",
                    HireDate = DateTime.Now,
                    PasswordChangedAt = DateTime.Now,
                    MustChangePassword = false,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                // Hash the password "Admin@123"
                adminUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123");

                _context.Users.Add(adminUser);
                await _context.SaveChangesAsync();

                // Now assign the admin role to this user if UserRoles table exists
                try
                {
                    var adminRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "admin");
                    
                    if (adminRole != null)
                    {
                        var userRole = new UserRole
                        {
                            UserId = adminUser.Id,
                            RoleId = adminRole.Id
                        };
                        
                        _context.UserRoles.Add(userRole);
                        await _context.SaveChangesAsync();
                    }
                }
                catch (Exception ex)
                {
                    // If UserRoles table doesn't exist, log and continue
                    Console.WriteLine($"Error assigning role to admin user: {ex.Message}");
                }
            }
        }
    }
}