using ApiContracts;
using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;
using WebAPI.Exceptions;
using WebAPI.Mapping;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class CommentsController : ControllerBase
{
    private readonly ICommentRepository commentRepo;
    private readonly IUserRepository userRepo;
    private readonly IPostRepository postRepo;

    public CommentsController(ICommentRepository commentRepo, IUserRepository userRepo, IPostRepository postRepo)
    {
        this.commentRepo = commentRepo;
        this.userRepo = userRepo;
        this.postRepo = postRepo;
    }

    // POST /comments
    [HttpPost]
    public async Task<ActionResult<CommentDto>> CreateComment([FromBody] CreateCommentDto request)
    {
        ValidateBody(request.Body);

        List<User> users = userRepo.GetMany().ToList();
        if (!users.Any(u => u.Id == request.UserId))
        {
            throw new BadRequestException($"User with ID '{request.UserId}' does not exist");
        }

        if (!postRepo.GetMany().AsEnumerable().Any(p => p.Id == request.PostId))
        {
            throw new BadRequestException($"Post with ID '{request.PostId}' does not exist");
        }

        Comment comment = new() { Body = request.Body, UserId = request.UserId, PostId = request.PostId };
        Comment created = await commentRepo.AddAsync(comment);

        CommentDto dto = DtoMapper.ToDto(created, users);
        return Created($"/Comments/{dto.Id}", dto);
    }

    // PUT /comments/{id}
    [HttpPut("{id:int}")]
    public async Task<ActionResult> UpdateComment([FromRoute] int id, [FromBody] UpdateCommentDto request)
    {
        ValidateBody(request.Body);

        Comment existing = await commentRepo.GetSingleAsync(id);
        existing.Body = request.Body;
        await commentRepo.UpdateAsync(existing);

        return NoContent();
    }

    // GET /comments/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<CommentDto>> GetSingleComment([FromRoute] int id)
    {
        Comment comment = await commentRepo.GetSingleAsync(id);
        List<User> users = userRepo.GetMany().ToList();
        return Ok(DtoMapper.ToDto(comment, users));
    }

    // GET /comments?userId=1&userName=anna&postId=2
    [HttpGet]
    public ActionResult<IEnumerable<CommentDto>> GetManyComments(
        [FromQuery] int? userId,
        [FromQuery] string? userName,
        [FromQuery] int? postId)
    {
        List<User> users = userRepo.GetMany().ToList();
        IEnumerable<Comment> comments = commentRepo.GetMany().AsEnumerable();

        if (userId.HasValue)
        {
            comments = comments.Where(c => c.UserId == userId.Value);
        }

        if (!string.IsNullOrWhiteSpace(userName))
        {
            HashSet<int> matchingUserIds = users
                .Where(u => u.UserName.Contains(userName, StringComparison.OrdinalIgnoreCase))
                .Select(u => u.Id)
                .ToHashSet();
            comments = comments.Where(c => matchingUserIds.Contains(c.UserId));
        }

        if (postId.HasValue)
        {
            comments = comments.Where(c => c.PostId == postId.Value);
        }

        List<CommentDto> result = comments.Select(c => DtoMapper.ToDto(c, users)).ToList();
        return Ok(result);
    }

    // DELETE /comments/{id}
    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteComment([FromRoute] int id)
    {
        await commentRepo.DeleteAsync(id);
        return NoContent();
    }

    private static void ValidateBody(string body)
    {
        if (string.IsNullOrWhiteSpace(body))
            throw new BadRequestException("Comment cannot be empty");
    }
}