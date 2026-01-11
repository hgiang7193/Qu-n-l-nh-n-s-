using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HRManagementSystem.Web.Data;
using HRManagementSystem.Web.Models;

namespace HRManagementSystem.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                // Check user's role to determine dashboard view
                var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
                var user = await _context.Users
                    .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (user != null && user.UserRoles.Any(ur => ur.Role.Name == "admin"))
                {
                    // Admin dashboard
                    var totalUsers = await _context.Users.CountAsync();
                    var totalProjects = await _context.Projects.CountAsync();
                    var activeEmployeesCount = await _context.Users.CountAsync(u => u.Status == "active");
                    var inactiveEmployeesCount = await _context.Users.CountAsync(u => u.Status == "inactive");

                    // Get attendance stats
                    var today = DateTime.Today;
                    var firstDay = new DateTime(today.Year, today.Month, 1);
                    var lastDay = new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month));

                    var monthlyAttendance = await _context.Attendances
                        .Where(a => a.Date >= firstDay && a.Date <= lastDay)
                        .ToListAsync();

                    var onTimeCount = monthlyAttendance.Count(a => a.Status == "on_time");
                    var lateCount = monthlyAttendance.Count(a => a.Status == "late");
                    var absentCount = monthlyAttendance.Count(a => a.Status == "absent");
                    var leaveCount = monthlyAttendance.Count(a => a.Status == "leave");

                    ViewBag.TotalUsers = totalUsers;
                    ViewBag.TotalProjects = totalProjects;
                    ViewBag.ActiveEmployeesCount = activeEmployeesCount;
                    ViewBag.InactiveEmployeesCount = inactiveEmployeesCount;
                    ViewBag.OnTimeCount = onTimeCount;
                    ViewBag.LateCount = lateCount;
                    ViewBag.AbsentCount = absentCount;
                    ViewBag.LeaveCount = leaveCount;

                    return View("AdminDashboard");
                }
                else
                {
                    // Regular employee dashboard
                    var userIdClaim = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
                    var employee = await _context.Users.FindAsync(userIdClaim);

                    // Get employee's assigned projects
                    var employeeAssignments = await _context.ProjectAssignments
                        .Include(pa => pa.Project)
                        .Where(pa => pa.EmployeeId == userIdClaim && pa.Status == "active")
                        .ToListAsync();

                    ViewBag.Employee = employee;
                    ViewBag.ActiveProjects = employeeAssignments.Count;

                    return View("EmployeeDashboard");
                }
            }
            
            // For non-authenticated users, show home page with stats
            ViewBag.TotalEmployees = await _context.Users.CountAsync();
            ViewBag.TotalProjects = await _context.Projects.CountAsync();
            ViewBag.TotalDepartments = await _context.Departments.CountAsync();
            ViewBag.TotalPositions = await _context.Positions.CountAsync();
            
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}