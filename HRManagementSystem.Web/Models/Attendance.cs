using System.ComponentModel.DataAnnotations;

namespace HRManagementSystem.Web.Models
{
    public class Attendance
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        
        [Required]
        public DateTime Date { get; set; }
        
        public DateTime? CheckIn { get; set; }
        public DateTime? CheckOut { get; set; }
        public string Status { get; set; }  // on_time, late, absent, leave, holiday
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public virtual User? Employee { get; set; }
    }
}