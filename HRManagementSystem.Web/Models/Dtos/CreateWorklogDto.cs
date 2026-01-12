using System;
using System.ComponentModel.DataAnnotations;

namespace HRManagementSystem.Web.Models.Dtos
{
    public class CreateWorklogDto
    {
        [Required]
        public int EmployeeId { get; set; }

        [Required]
        public int ProjectId { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        [Range(0.1, 24.0)]
        public double Hours { get; set; }

        public string Description { get; set; }
    }
}
