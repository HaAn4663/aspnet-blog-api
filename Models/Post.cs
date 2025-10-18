using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace BlogApi.Models
{
    public class Post
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string? Content { get; set; }

        public DateTime PublishedDate { get; set; }

        // Khóa ngoại tới IdentityUser
        [Required]
        public string AuthorId { get; set; }

        // Navigation property tới tác giả (CÁI NÀY BỊ THIẾU)
        public IdentityUser Author { get; set; }

        // Navigation property cho quan hệ nhiều-nhiều với Tag
        public ICollection<PostTag> PostTags { get; set; } = new List<PostTag>();
    }
}