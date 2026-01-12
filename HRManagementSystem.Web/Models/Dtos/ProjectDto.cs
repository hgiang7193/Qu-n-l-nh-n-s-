using System;

namespace HRManagementSystem.Web.Models.Dtos
{
    public class ProjectDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; }
        public string ProjectType { get; set; }
        public int? ProjectManagerId { get; set; }
    }
}