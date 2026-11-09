using ApiContracts;

namespace BlazorApp.Services;

public interface ICommentService
{
    public Task<CommentDto> AddCommentAsync(CreateCommentDto request);
    public Task UpdateCommentAsync(int id, UpdateCommentDto request);
    public Task<List<CommentDto>> GetCommentsForPostAsync(int postId);
    public Task DeleteCommentAsync(int id);
}
