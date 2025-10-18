using System.ComponentModel.DataAnnotations;

namespace BlogApi.Models
{
    public class Tag
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        // Navigation property cho quan hệ nhiều-nhiều với Post
        public ICollection<PostTag> PostTags { get; set; } = new List<PostTag>();
    }
}