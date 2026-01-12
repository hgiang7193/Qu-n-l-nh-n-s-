using System.ComponentModel.DataAnnotations;

namespace HRManagementSystem.Web.Models.Dtos
{
    public class CreateRolePermissionDto
    {
        [Required]
        public int RoleId { get; set; }

        [Required]
        public int PermissionId { get; set; }
    }
}
