using System.ComponentModel.DataAnnotations;

namespace HRManagementSystem.Web.Models
{
    public class ProjectAssignment
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public int EmployeeId { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Role { get; set; }
        
        [Required]
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Status { get; set; } = "active";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public virtual Project? Project { get; set; }
        public virtual User? Employee { get; set; }
    }
}