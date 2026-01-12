using System;
using System.ComponentModel.DataAnnotations;

namespace HRManagementSystem.Web.Models.Dtos
{
    public class UpdateUserDto
    {
        [Required]
        [EmailAddress]
        [StringLength(120)]
        public string Email { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        public string LastName { get; set; }

        public int? DepartmentId { get; set; }
        public int? PositionId { get; set; }
        public int? ManagerId { get; set; }
        public DateTime? HireDate { get; set; }
        public string Phone { get; set; }
        public string Notes { get; set; }
        public string Status { get; set; }
    }
}
