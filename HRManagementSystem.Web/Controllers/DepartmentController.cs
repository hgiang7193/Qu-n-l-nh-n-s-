using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HRManagementSystem.Web.Data;
using HRManagementSystem.Web.Models;

namespace HRManagementSystem.Web.Controllers
{
    public class DepartmentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DepartmentController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Department
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Index()
        {
            var departments = await _context.Departments
                .Include(d => d.Manager)
                .Include(d => d.Parent)
                .ToListAsync();
            return View(departments);
        }

        // GET: Department/Details/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var department = await _context.Departments
                .Include(d => d.Manager)
                .Include(d => d.Parent)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (department == null)
            {
                return NotFound();
            }

            return View(department);
        }

        // GET: Department/Create
        [Authorize(Roles = "admin")]
        public IActionResult Create()
        {
            ViewBag.Managers = _context.Users.ToList();
            ViewBag.ParentDepartments = _context.Departments.ToList();
            return View();
        }

        // POST: Department/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create([Bind("Name,Code,Description,ParentId,ManagerId,Status")] Department department)
        {
            if (ModelState.IsValid)
            {
                department.CreatedAt = DateTime.UtcNow;
                department.UpdatedAt = DateTime.UtcNow;
                _context.Add(department);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Managers = _context.Users.ToList();
            ViewBag.ParentDepartments = _context.Departments.ToList();
            return View(department);
        }

        // GET: Department/Edit/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var department = await _context.Departments.FindAsync(id);
            if (department == null)
            {
                return NotFound();
            }
            
            ViewBag.Managers = _context.Users.ToList();
            ViewBag.ParentDepartments = _context.Departments.Where(d => d.Id != id).ToList();
            return View(department);
        }

        // POST: Department/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Code,Description,ParentId,ManagerId,Status")] Department department)
        {
            if (id != department.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Get the existing department to preserve CreatedAt
                    var existingDepartment = await _context.Departments.FindAsync(id);
                    if (existingDepartment != null)
                    {
                        // Update only the fields from the form
                        existingDepartment.Name = department.Name;
                        existingDepartment.Code = department.Code;
                        existingDepartment.Description = department.Description;
                        existingDepartment.ParentId = department.ParentId;
                        existingDepartment.ManagerId = department.ManagerId;
                        existingDepartment.Status = department.Status;
                        existingDepartment.UpdatedAt = DateTime.UtcNow;
                        
                        _context.Update(existingDepartment);
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DepartmentExists(department.Id))
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
            ViewBag.Managers = _context.Users.ToList();
            ViewBag.ParentDepartments = _context.Departments.Where(d => d.Id != id).ToList();
            return View(department);
        }

        // GET: Department/Delete/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var department = await _context.Departments
                .Include(d => d.Manager)
                .Include(d => d.Parent)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (department == null)
            {
                return NotFound();
            }

            return View(department);
        }

        // POST: Department/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var department = await _context.Departments.FindAsync(id);
            if (department != null)
            {
                _context.Departments.Remove(department);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DepartmentExists(int id)
        {
            return _context.Departments.Any(e => e.Id == id);
        }
    }
}