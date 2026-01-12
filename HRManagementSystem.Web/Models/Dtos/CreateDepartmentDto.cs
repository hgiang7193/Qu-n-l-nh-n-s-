using System.ComponentModel.DataAnnotations;

namespace HRManagementSystem.Web.Models.Dtos
{
    public class CreateDepartmentDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(20)]
        public string Code { get; set; }

        public string Description { get; set; }
        public int? ParentId { get; set; }
        public int? ManagerId { get; set; }
    }
}