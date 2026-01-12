using System;
using System.ComponentModel.DataAnnotations;

namespace HRManagementSystem.Web.Models.Dtos
{
    public class UpdateAttendanceDto
    {
        [Required]
        public DateTime Date { get; set; }

        public DateTime? CheckIn { get; set; }
        public DateTime? CheckOut { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; }
    }
}
