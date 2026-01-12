using System.ComponentModel.DataAnnotations;

namespace HRManagementSystem.Web.Models.Dtos
{
    public class CreatePositionDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        public string Description { get; set; }
    }
}