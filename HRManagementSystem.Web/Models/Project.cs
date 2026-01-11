using System.ComponentModel.DataAnnotations;

namespace HRManagementSystem.Web.Models
{
    public class Project
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Name { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Code { get; set; }
        
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; } = "active";  // active, completed, cancelled
        public string ProjectType { get; set; } = "software";  // software, work, service
        public int? ProjectManagerId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public virtual User? ProjectManager { get; set; }
        public virtual ICollection<ProjectAssignment>? ProjectAssignments { get; set; }
        public virtual ICollection<Worklog>? Worklogs { get; set; }
    }
}