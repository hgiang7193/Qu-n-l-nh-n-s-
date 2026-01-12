using System;
using System.Collections.Generic;

namespace HRManagementSystem.Web.Models.Dtos
{
    public class DepartmentDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public int? ParentId { get; set; }
        public int? ManagerId { get; set; }
        public string Status { get; set; }
    }
}