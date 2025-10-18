using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlogApi.Data;
using BlogApi.Models;
using BlogApi.DTOs;
using System.Security.Claims;

namespace BlogApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public PostsController(AppDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/posts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PostSummaryDto>>> GetPosts()
        {
            var posts = await _context.Posts
                .Select(p => new PostSummaryDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    AuthorName = p.Author.UserName
                })
                .ToListAsync();

            return Ok(posts);
        }

        // GET: api/posts/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<PostDetailDto>> GetPost(int id)
        {
            var post = await _context.Posts
                .Include(p => p.PostTags).ThenInclude(pt => pt.Tag)
                .Where(p => p.Id == id)
                .Select(p => new PostDetailDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Content = p.Content,
                    PublishedDate = p.PublishedDate,
                    AuthorName = p.Author.UserName,
                    Tags = p.PostTags.Select(pt => pt.Tag.Name).ToList()
                })
                .FirstOrDefaultAsync();

            if (post == null)
            {
                return NotFound(new { Message = "Không tìm thấy bài viết." });
            }

            return Ok(post);
        }

        // POST: api/posts
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreatePost([FromBody] CreatePostDto createPostDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            var post = new Post
            {
                Title = createPostDto.Title,
                Content = createPostDto.Content,
                AuthorId = userId,
                PublishedDate = DateTime.UtcNow
            };
            
            await HandlePostTags(post, createPostDto.TagNames);

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPost), new { id = post.Id }, new { Message = "Tạo bài viết thành công!", PostId = post.Id });
        }

        // PUT: api/posts/{id}
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdatePost(int id, [FromBody] UpdatePostDto updatePostDto)
        {
            var post = await _context.Posts
                .Include(p => p.PostTags)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (post == null)
            {
                return NotFound(new { Message = "Không tìm thấy bài viết." });
            }

            if (!await IsUserAuthorized(post))
            {
                return Forbid();
            }

            post.Title = updatePostDto.Title;
            post.Content = updatePostDto.Content;

            await HandlePostTags(post, updatePostDto.TagNames, isUpdate: true);

            await _context.SaveChangesAsync();
            return Ok(new { Message = "Cập nhật bài viết thành công!" });
        }


        // DELETE: api/posts/{id}
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeletePost(int id)
        {
            var post = await _context.Posts.FindAsync(id);

            if (post == null)
            {
                return NotFound(new { Message = "Không tìm thấy bài viết." });
            }

            if (!await IsUserAuthorized(post))
            {
                return Forbid();
            }

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Đã xóa bài viết thành công!" });
        }

        // GET: api/posts/by-tag/{tagName}
        [HttpGet("by-tag/{tagName}")]
        public async Task<ActionResult<IEnumerable<PostSummaryDto>>> GetPostsByTag(string tagName)
        {
            var posts = await _context.Posts
                .Where(p => p.PostTags.Any(pt => pt.Tag.Name.ToLower() == tagName.ToLower()))
                .Select(p => new PostSummaryDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    AuthorName = p.Author.UserName
                })
                .ToListAsync();

            return Ok(posts);
        }

        // --- Private Helper Methods ---

        private async Task HandlePostTags(Post post, List<string> tagNames, bool isUpdate = false)
        {
            if (isUpdate)
            {
                post.PostTags.Clear();
            }

            if (tagNames == null || !tagNames.Any()) return;

            foreach (var tagName in tagNames)
            {
                var tagNameLower = tagName.Trim().ToLower();
                var existingTag = await _context.Tags.FirstOrDefaultAsync(t => t.Name.ToLower() == tagNameLower);

                if (existingTag == null)
                {
                    existingTag = new Tag { Name = tagName.Trim() };
                    _context.Tags.Add(existingTag);
                }
                
                post.PostTags.Add(new PostTag { Tag = existingTag });
            }
        }
        
        private async Task<bool> IsUserAuthorized(Post post)
        {
             var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
             var currentUser = await _userManager.FindByIdAsync(currentUserId);
             
             if (currentUser == null) return false;

             var isAdmin = await _userManager.IsInRoleAsync(currentUser, "Admin");

             return isAdmin || post.AuthorId == currentUserId;
        }
    }
}