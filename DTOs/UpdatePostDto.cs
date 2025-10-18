using System.ComponentModel.DataAnnotations;

namespace BlogApi.DTOs
{
    public class UpdatePostDto
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(255)]
        public string Title { get; set; }

        public string? Content { get; set; }

        public List<string> TagNames { get; set; } = new();
    }
}