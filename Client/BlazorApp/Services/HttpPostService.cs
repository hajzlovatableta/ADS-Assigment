using ApiContracts;

namespace BlazorApp.Services;

public class HttpPostService : IPostService
{
    private readonly HttpClient client;

    public HttpPostService(HttpClient client)
    {
        this.client = client;
    }

    public async Task<PostDto> AddPostAsync(CreatePostDto request)
    {
        HttpResponseMessage httpResponse = await client.PostAsJsonAsync("posts", request);
        return await HttpResponseHelper.ReadAsync<PostDto>(httpResponse);
    }

    public async Task UpdatePostAsync(int id, UpdatePostDto request)
    {
        HttpResponseMessage httpResponse = await client.PutAsJsonAsync($"posts/{id}", request);
        await HttpResponseHelper.EnsureSuccessAsync(httpResponse);
    }

    public async Task<PostDto> GetPostAsync(int id, bool includeComments = true)
    {
        HttpResponseMessage httpResponse = await client.GetAsync($"posts/{id}?includeComments={includeComments}");
        return await HttpResponseHelper.ReadAsync<PostDto>(httpResponse);
    }

    public async Task<List<PostDto>> GetPostsAsync(string? title = null, string? userName = null)
    {
        List<string> query = new();
        if (!string.IsNullOrWhiteSpace(title)) query.Add($"title={Uri.EscapeDataString(title)}");
        if (!string.IsNullOrWhiteSpace(userName)) query.Add($"userName={Uri.EscapeDataString(userName)}");

        string uri = query.Count == 0 ? "posts" : "posts?" + string.Join("&", query);
        HttpResponseMessage httpResponse = await client.GetAsync(uri);
        return await HttpResponseHelper.ReadAsync<List<PostDto>>(httpResponse);
    }

    public async Task DeletePostAsync(int id)
    {
        HttpResponseMessage httpResponse = await client.DeleteAsync($"posts/{id}");
        await HttpResponseHelper.EnsureSuccessAsync(httpResponse);
    }
}
