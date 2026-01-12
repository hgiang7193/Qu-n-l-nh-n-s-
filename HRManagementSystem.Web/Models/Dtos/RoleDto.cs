using System.ComponentModel.DataAnnotations;

namespace HRManagementSystem.Web.Models.Dtos;

public class RoleDto
{
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string Name { get; set; }

    [StringLength(255)]
    public string? Description { get; set; }
}