using System.ComponentModel.DataAnnotations;

namespace HRManagementSystem.Web.Models
{
    public class Worklog
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public int ProjectId { get; set; }
        
        [Required]
        public DateTime Date { get; set; }
        
        [Required]
        public double Hours { get; set; }
        
        public string Description { get; set; }
        public string Status { get; set; } = "submitted";  // submitted, approved, rejected
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public virtual User? Employee { get; set; }
        public virtual Project? Project { get; set; }
    }
}