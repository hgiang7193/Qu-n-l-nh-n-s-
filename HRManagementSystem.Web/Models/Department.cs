using System.ComponentModel.DataAnnotations;

namespace HRManagementSystem.Web.Models
{
    public class Department
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        
        [Required]
        [StringLength(20)]
        public string Code { get; set; }
        
        public string Description { get; set; }
        public int? ParentId { get; set; }
        public int? ManagerId { get; set; }
        public string Status { get; set; } = "active";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public virtual Department? Parent { get; set; }
        public virtual User? Manager { get; set; }
        public virtual ICollection<Department>? Children { get; set; }
        public virtual ICollection<User>? Employees { get; set; }
    }
}