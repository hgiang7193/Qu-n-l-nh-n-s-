using System;
using System.ComponentModel.DataAnnotations;

namespace HRManagementSystem.Web.Models.Dtos
{
    public class CreateProjectAssignmentDto
    {
        [Required]
        public int ProjectId { get; set; }

        [Required]
        public int EmployeeId { get; set; }

        [Required]
        [StringLength(100)]
        public string Role { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }
    }
}
