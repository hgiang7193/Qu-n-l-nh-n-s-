using System.ComponentModel.DataAnnotations;

namespace HRManagementSystem.Web.Models.Dtos
{
    public class CreateUserRoleDto
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public int RoleId { get; set; }
    }
}
