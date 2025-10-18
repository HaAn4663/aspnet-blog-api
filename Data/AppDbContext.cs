using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BlogApi.Models;

namespace BlogApi.Data
{
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Post> Posts { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<PostTag> PostTags { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Cấu hình này RẤT QUAN TRỌNG, phải gọi đầu tiên
            base.OnModelCreating(builder);

            // --- Cấu hình cho quan hệ nhiều-nhiều giữa Post và Tag ---

            // 1. Định nghĩa khóa chính kết hợp (composite key) cho bảng PostTag
            builder.Entity<PostTag>()
                .HasKey(pt => new { pt.PostId, pt.TagId });

            // 2. Thiết lập mối quan hệ từ PostTag -> Post
            builder.Entity<PostTag>()
                .HasOne(pt => pt.Post)
                .WithMany(p => p.PostTags)
                .HasForeignKey(pt => pt.PostId);

            // 3. Thiết lập mối quan hệ từ PostTag -> Tag
            builder.Entity<PostTag>()
                .HasOne(pt => pt.Tag)
                .WithMany(t => t.PostTags)
                .HasForeignKey(pt => pt.TagId);
        }
    }
}