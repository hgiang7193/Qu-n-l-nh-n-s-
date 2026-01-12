using System;

namespace HRManagementSystem.Web.Models.Dtos
{
    public class EmployeeDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string EmployeeCode { get; set; }
        public int? DepartmentId { get; set; }
        public int? PositionId { get; set; }
        public int? ManagerId { get; set; }
        public DateTime? HireDate { get; set; }
        public string Phone { get; set; }
        public string Status { get; set; }
    }
}