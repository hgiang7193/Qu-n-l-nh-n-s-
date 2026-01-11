using System.ComponentModel.DataAnnotations;

namespace HRManagementSystem.Web.Models
{
    public class Shift
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        
        [StringLength(500)]
        public string Description { get; set; }
        
        public TimeSpan StartTime { get; set; }
        
        public TimeSpan EndTime { get; set; }
        
        [StringLength(20)]
        public string Status { get; set; } = "Active"; // Active, Inactive
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        

    }
}