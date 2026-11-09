using ApiContracts;

namespace BlazorApp.Services;

public interface IPostService
{
    public Task<PostDto> AddPostAsync(CreatePostDto request);
    public Task UpdatePostAsync(int id, UpdatePostDto request);
    public Task<PostDto> GetPostAsync(int id, bool includeComments = true);
    public Task<List<PostDto>> GetPostsAsync(string? title = null, string? userName = null);
    public Task DeletePostAsync(int id);
}
