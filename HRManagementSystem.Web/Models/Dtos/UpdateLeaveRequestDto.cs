using System;
using System.ComponentModel.DataAnnotations;

namespace HRManagementSystem.Web.Models.Dtos
{
    public class UpdateLeaveRequestDto
    {
        [Required]
        [StringLength(50)]
        public string LeaveType { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public string Reason { get; set; }

        [StringLength(50)]
        public string Status { get; set; }  // pending, approved, rejected
    }
}
