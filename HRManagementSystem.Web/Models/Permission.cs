using System.ComponentModel.DataAnnotations;

namespace HRManagementSystem.Web.Models
{
    public class Permission
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; }  // e.g., 'can_view_employees', 'can_edit_projects'
        
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public virtual ICollection<RolePermission>? RolePermissions { get; set; }
    }
}