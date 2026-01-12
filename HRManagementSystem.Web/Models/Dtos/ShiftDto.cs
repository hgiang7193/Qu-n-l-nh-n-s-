using System;

namespace HRManagementSystem.Web.Models.Dtos
{
    public class ShiftDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Status { get; set; }
    }
}