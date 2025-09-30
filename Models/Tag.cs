using System.Collections.Generic;

namespace AspNetBlogApi.Models
{
    public class Tag
    {
        public int Id { get; set; }       // Khóa chính
        public string Name { get; set; }  // Tên thẻ

        // Quan hệ nhiều-nhiều với Post
        public ICollection<PostTag> PostTags { get; set; }
    }
}
