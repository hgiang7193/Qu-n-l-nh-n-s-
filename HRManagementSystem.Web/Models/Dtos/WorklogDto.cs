using System;

namespace HRManagementSystem.Web.Models.Dtos
{
    public class WorklogDto
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public DateTime Date { get; set; }
        public double Hours { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
