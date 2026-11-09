using ApiContracts;

namespace BlazorApp.Services;

public class HttpCommentService : ICommentService
{
    private readonly HttpClient client;

    public HttpCommentService(HttpClient client)
    {
        this.client = client;
    }

    public async Task<CommentDto> AddCommentAsync(CreateCommentDto request)
    {
        HttpResponseMessage httpResponse = await client.PostAsJsonAsync("comments", request);
        return await HttpResponseHelper.ReadAsync<CommentDto>(httpResponse);
    }

    public async Task UpdateCommentAsync(int id, UpdateCommentDto request)
    {
        HttpResponseMessage httpResponse = await client.PutAsJsonAsync($"comments/{id}", request);
        await HttpResponseHelper.EnsureSuccessAsync(httpResponse);
    }

    public async Task<List<CommentDto>> GetCommentsForPostAsync(int postId)
    {
        HttpResponseMessage httpResponse = await client.GetAsync($"posts/{postId}/comments");
        return await HttpResponseHelper.ReadAsync<List<CommentDto>>(httpResponse);
    }

    public async Task DeleteCommentAsync(int id)
    {
        HttpResponseMessage httpResponse = await client.DeleteAsync($"comments/{id}");
        await HttpResponseHelper.EnsureSuccessAsync(httpResponse);
    }
}
