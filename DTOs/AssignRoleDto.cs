using System.ComponentModel.DataAnnotations;

namespace BlogApi.DTOs
{
    public class AssignRoleDto
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string RoleName { get; set; }
    }
}