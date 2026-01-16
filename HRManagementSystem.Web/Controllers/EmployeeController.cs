using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HRManagementSystem.Web.Data;
using HRManagementSystem.Web.Models;

namespace HRManagementSystem.Web.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EmployeeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Employee
        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("admin"))
            {
                // Admin can see all employees
                var employees = await _context.Users
                    .Include(u => u.Department)
                    .Include(u => u.Position)
                    .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                    .ToListAsync();
                return View(employees);
            }
            else
            {
                // Regular employee can only see their own information
                var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
                var employee = await _context.Users
                    .Include(u => u.Department)
                    .Include(u => u.Position)
                    .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                    .Where(u => u.Id == userId)
                    .ToListAsync();
                return View(employee);
            }
        }

        // GET: Employee/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Users
                .Include(u => u.Department)
                .Include(u => u.Position)
                .Include(u => u.Manager)
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (employee == null)
            {
                return NotFound();
            }

            // Allow admin to see any employee, but regular users can only see their own profile
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (!User.IsInRole("admin") && employee.Id != userId)
            {
                return Forbid(); // Return forbidden if not admin and not their own profile
            }

            // Include project assignments for the employee
            employee = await _context.Users
                .Include(u => u.Department)
                .Include(u => u.Position)
                .Include(u => u.Manager)
                .Include(u => u.ProjectAssignments)
                    .ThenInclude(pa => pa.Project)
                .FirstOrDefaultAsync(m => m.Id == id);

            return View(employee);
        }

        // GET: Employee/Create
        [Authorize(Roles = "admin")]
        public IActionResult Create()
        {
            ViewBag.Departments = _context.Departments
                .Select(d => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem 
                { 
                    Value = d.Id.ToString(), 
                    Text = d.Name 
                })
                .ToList();
            
            ViewBag.Positions = _context.Positions
                .Select(p => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem 
                { 
                    Value = p.Id.ToString(), 
                    Text = p.Name 
                })
                .ToList();
            
            ViewBag.Managers = _context.Users
                .Select(m => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem 
                { 
                    Value = m.Id.ToString(), 
                    Text = $"{m.FirstName} {m.LastName}" 
                })
                .ToList();
            
            return View();
        }

        // POST: Employee/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create([Bind("Username,Email,FirstName,LastName,EmployeeCode,DepartmentId,PositionId,ManagerId,HireDate,Phone,Status")] User user)
        {
            if (ModelState.IsValid)
            {
                // Generate username as abbreviation of first and last name
                var username = (user.FirstName[0] + user.LastName).ToLower().Replace(" ", "");
                
                user.Username = username;
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(username + "123"); // Default password: name abbreviation + 123
                user.CreatedAt = DateTime.UtcNow;
                user.UpdatedAt = DateTime.UtcNow;
                
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Departments = _context.Departments
                .Select(d => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem 
                { 
                    Value = d.Id.ToString(), 
                    Text = d.Name 
                })
                .ToList();
            
            ViewBag.Positions = _context.Positions
                .Select(p => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem 
                { 
                    Value = p.Id.ToString(), 
                    Text = p.Name 
                })
                .ToList();
            
            ViewBag.Managers = _context.Users
                .Select(m => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem 
                { 
                    Value = m.Id.ToString(), 
                    Text = $"{m.FirstName} {m.LastName}" 
                })
                .ToList();
            
            return View(user);
        }

        // GET: Employee/Edit/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            ViewBag.Departments = _context.Departments
                .Select(d => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem 
                { 
                    Value = d.Id.ToString(), 
                    Text = d.Name 
                })
                .ToList();
            
            ViewBag.Positions = _context.Positions
                .Select(p => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem 
                { 
                    Value = p.Id.ToString(), 
                    Text = p.Name 
                })
                .ToList();
            
            ViewBag.Managers = _context.Users
                .Select(m => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem 
                { 
                    Value = m.Id.ToString(), 
                    Text = $"{m.FirstName} {m.LastName}" 
                })
                .ToList();
            
            return View(user);
        }

        // POST: Employee/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Username,Email,FirstName,LastName,EmployeeCode,DepartmentId,PositionId,ManagerId,HireDate,Phone,Notes,Status")] User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    user.UpdatedAt = DateTime.UtcNow;
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
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

            ViewBag.Departments = _context.Departments
                .Select(d => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem 
                { 
                    Value = d.Id.ToString(), 
                    Text = d.Name 
                })
                .ToList();
            
            ViewBag.Positions = _context.Positions
                .Select(p => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem 
                { 
                    Value = p.Id.ToString(), 
                    Text = p.Name 
                })
                .ToList();
            
            ViewBag.Managers = _context.Users
                .Select(m => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem 
                { 
                    Value = m.Id.ToString(), 
                    Text = $"{m.FirstName} {m.LastName}" 
                })
                .ToList();
            
            return View(user);
        }

        // GET: Employee/Delete/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.Department)
                .Include(u => u.Position)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Employee/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}