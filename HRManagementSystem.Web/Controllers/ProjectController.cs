using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HRManagementSystem.Web.Data;
using HRManagementSystem.Web.Models;

namespace HRManagementSystem.Web.Controllers
{
    public class ProjectController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProjectController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Project
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Index()
        {
            var projects = await _context.Projects
                .Include(p => p.ProjectManager)
                .ToListAsync();

            return View(projects);
        }

        // GET: Project/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Projects
                .Include(p => p.ProjectManager)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (project == null)
            {
                return NotFound();
            }
            
            // Allow admin to see any project, but regular users can only see projects they are assigned to
            if (!User.IsInRole("admin"))
            {
                var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
                var isAssignedToProject = await _context.ProjectAssignments
                    .AnyAsync(pa => pa.ProjectId == id && pa.EmployeeId == userId);
                    
                if (!isAssignedToProject)
                {
                    return Forbid();
                }
            }

            return View(project);
        }

        // GET: Project/Create
        [Authorize(Roles = "admin")]
        public IActionResult Create()
        {
            ViewBag.ProjectManagers = _context.Users.ToList();
            return View();
        }

        // POST: Project/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create([Bind("Name,Code,Description,StartDate,EndDate,Status,ProjectType,ProjectManagerId")] Project project)
        {
            if (ModelState.IsValid)
            {
                _context.Add(project);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.ProjectManagers = _context.Users.ToList();
            return View(project);
        }

        // GET: Project/Edit/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            ViewBag.ProjectManagers = _context.Users.ToList();
            return View(project);
        }

        // POST: Project/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Code,Description,StartDate,EndDate,Status,ProjectType,ProjectManagerId")] Project project)
        {
            if (id != project.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    project.UpdatedAt = DateTime.UtcNow;
                    _context.Update(project);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectExists(project.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.ProjectManagers = _context.Users.ToList();
            return View(project);
        }

        // GET: Project/Delete/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Projects
                .Include(p => p.ProjectManager)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // POST: Project/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project != null)
            {
                _context.Projects.Remove(project);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Project/MyProjects (shows projects assigned to the current user)
        public async Task<IActionResult> MyProjects()
        {
            if (User.IsInRole("admin"))
            {
                // Admin can see all projects
                var projects = await _context.Projects.ToListAsync();
                return View(projects);
            }
            else
            {
                // Regular employee can only see projects they are assigned to
                var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
                var assignedProjectIds = _context.ProjectAssignments
                    .Where(pa => pa.EmployeeId == userId)
                    .Select(pa => pa.ProjectId)
                    .Distinct()
                    .ToList();
                
                var projects = await _context.Projects
                    .Where(p => assignedProjectIds.Contains(p.Id))
                    .ToListAsync();
                
                return View("MyProjects", projects);
            }
        }

        // GET: Project/AssignEmployees/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> AssignEmployees(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            // Get all employees not already assigned to this project
            var assignedEmployeeIds = _context.ProjectAssignments
                .Where(pa => pa.ProjectId == id)
                .Select(pa => pa.EmployeeId)
                .ToList();

            var availableEmployees = await _context.Users
                .Where(u => !assignedEmployeeIds.Contains(u.Id))
                .ToListAsync();

            ViewBag.Project = project;
            ViewBag.AvailableEmployees = availableEmployees;
            
            // Also get current assignments for this project
            var currentAssignments = await _context.ProjectAssignments
                .Include(pa => pa.Employee)
                .Where(pa => pa.ProjectId == id)
                .ToListAsync();

            return View(currentAssignments);
        }

        // POST: Project/AssignEmployee
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> AssignEmployee(int projectId, int employeeId, string role, DateTime startDate)
        {
            var existingAssignment = await _context.ProjectAssignments
                .FirstOrDefaultAsync(pa => pa.ProjectId == projectId && pa.EmployeeId == employeeId);

            if (existingAssignment == null)
            {
                var assignment = new ProjectAssignment
                {
                    ProjectId = projectId,
                    EmployeeId = employeeId,
                    Role = role,
                    StartDate = startDate,
                    Status = "active",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.ProjectAssignments.Add(assignment);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(AssignEmployees), new { id = projectId });
        }

        // POST: Project/RemoveAssignment/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> RemoveAssignment(int id)
        {
            var assignment = await _context.ProjectAssignments.FindAsync(id);
            if (assignment != null)
            {
                // Check if assignment has any worklogs (tasks)
                var worklogsExist = await _context.Worklogs
                    .AnyAsync(w => w.ProjectId == assignment.ProjectId && w.EmployeeId == assignment.EmployeeId);

                if (!worklogsExist)
                {
                    _context.ProjectAssignments.Remove(assignment);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    TempData["ErrorMessage"] = "Không thể gỡ nhân viên vì đã có nhiệm vụ được giao trong dự án.";
                }
            }

            return RedirectToAction(nameof(AssignEmployees), new { id = assignment?.ProjectId });
        }

        private bool ProjectExists(int id)
        {
            return _context.Projects.Any(e => e.Id == id);
        }
    }
}