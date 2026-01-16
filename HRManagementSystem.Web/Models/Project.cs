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
        
        [Required(ErrorMessage = "Ngày bắt đầu là bắt buộc.")]
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        
        [Required(ErrorMessage = "Trạng thái là bắt buộc.")]
        [StringLength(20, ErrorMessage = "Trạng thái không được vượt quá 20 ký tự.")]
        public string Status { get; set; } = "active";  // active, completed, cancelled
        
        [Required(ErrorMessage = "Loại dự án là bắt buộc.")]
        [StringLength(20, ErrorMessage = "Loại dự án không được vượt quá 20 ký tự.")]
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