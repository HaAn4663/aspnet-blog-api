namespace BlogApi.Models
{
    public class PostTag
    {
        // Khóa chính kết hợp (Composite Primary Key)
        public int PostId { get; set; }
        public int TagId { get; set; }

        // Navigation properties
        public Post Post { get; set; }
        public Tag Tag { get; set; }
    }
}