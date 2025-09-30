using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace AspNetBlogApi.Models
{
    public class Post
    {
        public int Id { get; set; }                 // Khóa chính
        public string Title { get; set; }           // Tiêu đề
        public string Content { get; set; }         // Nội dung
        public DateTime PublishedDate { get; set; } // Ngày đăng

        // Quan hệ với IdentityUser
        public string AuthorId { get; set; }
        public IdentityUser Author { get; set; }

        // Quan hệ nhiều-nhiều với Tag
        public ICollection<PostTag> PostTags { get; set; }
    }
}
