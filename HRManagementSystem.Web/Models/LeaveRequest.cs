using System.ComponentModel.DataAnnotations;

namespace HRManagementSystem.Web.Models
{
    public class LeaveRequest
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        
        [Required]
        [StringLength(50)]
        public string LeaveType { get; set; }  // annual, sick, personal, etc.
        
        [Required]
        public DateTime StartDate { get; set; }
        
        [Required]
        public DateTime EndDate { get; set; }
        
        public string Reason { get; set; }
        public string Status { get; set; } = "pending";  // pending, approved, rejected
        public int? ApprovedBy { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public virtual User? Employee { get; set; }
        public virtual User? ApprovedByUser { get; set; }
    }
}