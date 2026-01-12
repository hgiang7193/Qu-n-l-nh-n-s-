using System;
using System.ComponentModel.DataAnnotations;

namespace HRManagementSystem.Web.Models.Dtos
{
    public class UpdateWorklogDto
    {
        [Required]
        public DateTime Date { get; set; }

        [Required]
        [Range(0.1, 24.0)]
        public double Hours { get; set; }

        public string Description { get; set; }

        [StringLength(50)]
        public string Status { get; set; }  // submitted, approved, rejected
    }
}
