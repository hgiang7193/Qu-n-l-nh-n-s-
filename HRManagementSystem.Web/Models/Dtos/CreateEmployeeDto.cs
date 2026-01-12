using System;
using System.ComponentModel.DataAnnotations;

namespace HRManagementSystem.Web.Models.Dtos
{
    public class CreateEmployeeDto
    {
        [Required]
        [StringLength(80)]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(120)]
        public string Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        public string LastName { get; set; }

        [Required]
        [StringLength(20)]
        public string EmployeeCode { get; set; }

        public int? DepartmentId { get; set; }
        public int? PositionId { get; set; }
        public int? ManagerId { get; set; }
        public DateTime? HireDate { get; set; }
        public string Phone { get; set; }
    }
}