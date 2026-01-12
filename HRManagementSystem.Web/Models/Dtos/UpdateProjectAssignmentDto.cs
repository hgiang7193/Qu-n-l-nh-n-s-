using System;
using System.ComponentModel.DataAnnotations;

namespace HRManagementSystem.Web.Models.Dtos
{
    public class UpdateProjectAssignmentDto
    {
        [Required]
        [StringLength(100)]
        public string Role { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        [StringLength(50)]
        public string Status { get; set; }
    }
}
