namespace BlogApi.DTOs
{
    public class PostDetailDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Content { get; set; }
        public DateTime PublishedDate { get; set; }
        public string AuthorName { get; set; }
        public List<string> Tags { get; set; } = new();
    }
}