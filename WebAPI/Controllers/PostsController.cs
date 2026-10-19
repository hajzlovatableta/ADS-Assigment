using ApiContracts;
using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;
using WebAPI.Exceptions;
using WebAPI.Mapping;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class PostsController : ControllerBase
{
    private readonly IPostRepository postRepo;
    private readonly IUserRepository userRepo;
    private readonly ICommentRepository commentRepo;

    public PostsController(IPostRepository postRepo, IUserRepository userRepo, ICommentRepository commentRepo)
    {
        this.postRepo = postRepo;
        this.userRepo = userRepo;
        this.commentRepo = commentRepo;
    }

    // POST /posts
    [HttpPost]
    public async Task<ActionResult<PostDto>> CreatePost([FromBody] CreatePostDto request)
    {
        ValidatePost(request.Title, request.Body);

        List<User> users = userRepo.GetMany().ToList();
        if (!users.Any(u => u.Id == request.UserId))
        {
            throw new BadRequestException($"User with ID '{request.UserId}' does not exist");
        }

        Post post = new() { Title = request.Title, Body = request.Body, UserId = request.UserId };
        Post created = await postRepo.AddAsync(post);

        PostDto dto = DtoMapper.ToDto(created, users);
        return Created($"/Posts/{dto.Id}", dto);
    }

    // PUT /posts/{id}
    [HttpPut("{id:int}")]
    public async Task<ActionResult> UpdatePost([FromRoute] int id, [FromBody] UpdatePostDto request)
    {
        ValidatePost(request.Title, request.Body);

        Post existing = await postRepo.GetSingleAsync(id);
        existing.Title = request.Title;
        existing.Body = request.Body;
        await postRepo.UpdateAsync(existing);

        return NoContent();
    }

    // GET /posts/{id}?includeComments=true
    [HttpGet("{id:int}")]
    public async Task<ActionResult<PostDto>> GetSinglePost([FromRoute] int id, [FromQuery] bool includeComments = false)
    {
        Post post = await postRepo.GetSingleAsync(id);
        List<User> users = userRepo.GetMany().ToList();

        List<CommentDto>? comments = null;
        if (includeComments)
        {
            comments = commentRepo.GetMany().AsEnumerable()
                .Where(c => c.PostId == id)
                .Select(c => DtoMapper.ToDto(c, users))
                .ToList();
        }

        return Ok(DtoMapper.ToDto(post, users, comments));
    }

    // GET /posts?title=hello&userId=1&userName=anna
    [HttpGet]
    public ActionResult<IEnumerable<PostDto>> GetManyPosts(
        [FromQuery] string? title,
        [FromQuery] int? userId,
        [FromQuery] string? userName)
    {
        List<User> users = userRepo.GetMany().ToList();
        IEnumerable<Post> posts = postRepo.GetMany().AsEnumerable();

        if (!string.IsNullOrWhiteSpace(title))
        {
            posts = posts.Where(p => p.Title.Contains(title, StringComparison.OrdinalIgnoreCase));
        }

        if (userId.HasValue)
        {
            posts = posts.Where(p => p.UserId == userId.Value);
        }

        if (!string.IsNullOrWhiteSpace(userName))
        {
            HashSet<int> matchingUserIds = users
                .Where(u => u.UserName.Contains(userName, StringComparison.OrdinalIgnoreCase))
                .Select(u => u.Id)
                .ToHashSet();
            posts = posts.Where(p => matchingUserIds.Contains(p.UserId));
        }

        List<PostDto> result = posts.Select(p => DtoMapper.ToDto(p, users)).ToList();
        return Ok(result);
    }

    // GET /posts/{postId}/comments
    [HttpGet("{postId:int}/comments")]
    public async Task<ActionResult<IEnumerable<CommentDto>>> GetCommentsForPost([FromRoute] int postId)
    {
        await postRepo.GetSingleAsync(postId); // 404 if the post does not exist
        List<User> users = userRepo.GetMany().ToList();

        List<CommentDto> comments = commentRepo.GetMany().AsEnumerable()
            .Where(c => c.PostId == postId)
            .Select(c => DtoMapper.ToDto(c, users))
            .ToList();

        return Ok(comments);
    }

    // DELETE /posts/{id}  (also deletes the post's comments)
    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeletePost([FromRoute] int id)
    {
        await postRepo.DeleteAsync(id);

        List<int> commentIds = commentRepo.GetMany().AsEnumerable()
            .Where(c => c.PostId == id)
            .Select(c => c.Id)
            .ToList();

        foreach (int commentId in commentIds)
        {
            await commentRepo.DeleteAsync(commentId);
        }

        return NoContent();
    }

    private static void ValidatePost(string title, string body)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new BadRequestException("Title cannot be empty");
        if (string.IsNullOrWhiteSpace(body))
            throw new BadRequestException("Body cannot be empty");
    }
}