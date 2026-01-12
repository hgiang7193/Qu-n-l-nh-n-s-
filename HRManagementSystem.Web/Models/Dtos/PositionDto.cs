using System;

namespace HRManagementSystem.Web.Models.Dtos
{
    public class PositionDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
    }
}