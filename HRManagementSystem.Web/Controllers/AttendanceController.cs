using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HRManagementSystem.Web.Data;
using HRManagementSystem.Web.Models;

namespace HRManagementSystem.Web.Controllers
{
    public class AttendanceController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AttendanceController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Attendance
        [Authorize]
        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("admin"))
            {
                // Admin can see all attendance records
                var attendances = await _context.Attendances
                    .Include(a => a.Employee)
                    .ToListAsync();
                return View(attendances);
            }
            else
            {
                // Regular employee can only see their own attendance
                var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
                var employeeAttendances = await _context.Attendances
                    .Include(a => a.Employee)
                    .Where(a => a.EmployeeId == userId)
                    .ToListAsync();
                return View(employeeAttendances);
            }
        }

        // GET: Attendance/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var attendance = await _context.Attendances
                .Include(a => a.Employee)
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (attendance == null)
            {
                return NotFound();
            }

            // Allow admin to see any attendance record, but regular users can only see their own
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (!User.IsInRole("admin") && attendance.EmployeeId != userId)
            {
                return Forbid(); // Return forbidden if not admin and not their own record
            }

            return View(attendance);
        }

        // GET: Attendance/Create - For employee to check in/out automatically
        [Authorize]
        public IActionResult Create()
        {
            // Only show form for admin to manually add attendance
            if (!User.IsInRole("admin"))
            {
                return View(); // Show the simple check-in/out view for employees
            }

            // For admin, show the full form
            ViewBag.Employees = _context.Users.ToList();
            return View("CreateManual"); // Separate view for admin to manually add
        }

        // POST: Attendance/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("EmployeeId,Date,CheckIn,CheckOut,Status")] Attendance attendance)
        {
            // Admin only for manual attendance creation
            if (!User.IsInRole("admin") && attendance.EmployeeId != int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0"))
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                attendance.CreatedAt = DateTime.UtcNow;
                _context.Add(attendance);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            
            if (User.IsInRole("admin"))
            {
                ViewBag.Employees = _context.Users.ToList();
                return View("CreateManual", attendance);
            }
            
            return View(attendance);
        }

        // GET: Attendance/Edit/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var attendance = await _context.Attendances.FindAsync(id);
            if (attendance == null)
            {
                return NotFound();
            }
            
            ViewBag.Employees = _context.Users.ToList();
            return View(attendance);
        }

        // POST: Attendance/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,EmployeeId,Date,CheckIn,CheckOut,Status,CreatedAt")] Attendance attendance)
        {
            if (id != attendance.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(attendance);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AttendanceExists(attendance.Id))
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
            ViewBag.Employees = _context.Users.ToList();
            return View(attendance);
        }

        // GET: Attendance/Delete/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var attendance = await _context.Attendances
                .Include(a => a.Employee)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (attendance == null)
            {
                return NotFound();
            }

            return View(attendance);
        }

        // POST: Attendance/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var attendance = await _context.Attendances.FindAsync(id);
            if (attendance != null)
            {
                _context.Attendances.Remove(attendance);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST: Attendance/CheckIn - Employee checks in
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> CheckIn()
        {
            var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out var userId))
            {
                TempData["Error"] = "Không thể xác định người dùng. Vui lòng đăng nhập lại.";
                return RedirectToAction(nameof(Index));
            }

            // Verify user exists in database
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                TempData["Error"] = "Người dùng không tồn tại trong hệ thống.";
                return RedirectToAction(nameof(Index));
            }

            var today = DateTime.Today;

            // Check if already checked in today
            var existingAttendance = await _context.Attendances
                .FirstOrDefaultAsync(a => a.EmployeeId == userId && a.Date == today);

            if (existingAttendance != null && existingAttendance.CheckIn != null)
            {
                TempData["Message"] = "Bạn đã chấm công vào hôm nay rồi.";
                return RedirectToAction(nameof(Index));
            }

            var currentDateTime = DateTime.Now;
            TimeSpan currentTime = currentDateTime.TimeOfDay;
            TimeSpan workStartTime = new TimeSpan(8, 0, 0); // 8:00 AM
            
            string status = currentTime <= workStartTime ? "on_time" : "late";

            if (existingAttendance == null)
            {
                var attendance = new Attendance
                {
                    EmployeeId = userId,
                    Date = today,
                    CheckIn = currentDateTime,
                    Status = status,
                    CreatedAt = DateTime.UtcNow
                };
                _context.Attendances.Add(attendance);
            }
            else
            {
                existingAttendance.CheckIn = currentDateTime;
                existingAttendance.Status = status;
                _context.Update(existingAttendance);
            }

            await _context.SaveChangesAsync();
            TempData["Message"] = $"Chấm công vào lúc {currentTime:HH:mm:ss}. Trạng thái: {(status == "on_time" ? "Đúng giờ" : "Trễ")}";
            return RedirectToAction(nameof(Index));
        }

        // POST: Attendance/CheckOut - Employee checks out
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> CheckOut()
        {
            var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out var userId))
            {
                TempData["Error"] = "Không thể xác định người dùng. Vui lòng đăng nhập lại.";
                return RedirectToAction(nameof(Index));
            }

            // Verify user exists in database
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                TempData["Error"] = "Người dùng không tồn tại trong hệ thống.";
                return RedirectToAction(nameof(Index));
            }

            var today = DateTime.Today;

            var attendance = await _context.Attendances
                .FirstOrDefaultAsync(a => a.EmployeeId == userId && a.Date == today);

            if (attendance == null || attendance.CheckIn == null)
            {
                TempData["Message"] = "Bạn chưa chấm công vào hôm nay.";
                return RedirectToAction(nameof(Index));
            }

            if (attendance.CheckOut != null)
            {
                TempData["Message"] = "Bạn đã chấm công ra hôm nay rồi.";
                return RedirectToAction(nameof(Index));
            }

            var currentDateTime = DateTime.Now;
            attendance.CheckOut = currentDateTime;
            _context.Update(attendance);
            await _context.SaveChangesAsync();

            TempData["Message"] = $"Chấm công ra lúc {currentDateTime:HH:mm:ss}";
            return RedirectToAction(nameof(Index));
        }

        private bool AttendanceExists(int id)
        {
            return _context.Attendances.Any(e => e.Id == id);
        }
    }
}