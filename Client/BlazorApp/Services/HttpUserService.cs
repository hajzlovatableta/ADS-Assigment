using ApiContracts;

namespace BlazorApp.Services;

public class HttpUserService : IUserService
{
    private readonly HttpClient client;

    public HttpUserService(HttpClient client)
    {
        this.client = client;
    }

    public async Task<UserDto> AddUserAsync(CreateUserDto request)
    {
        HttpResponseMessage httpResponse = await client.PostAsJsonAsync("users", request);
        return await HttpResponseHelper.ReadAsync<UserDto>(httpResponse);
    }

    public async Task UpdateUserAsync(int id, UpdateUserDto request)
    {
        HttpResponseMessage httpResponse = await client.PutAsJsonAsync($"users/{id}", request);
        await HttpResponseHelper.EnsureSuccessAsync(httpResponse);
    }

    public async Task<UserDto> GetUserAsync(int id)
    {
        HttpResponseMessage httpResponse = await client.GetAsync($"users/{id}");
        return await HttpResponseHelper.ReadAsync<UserDto>(httpResponse);
    }

    public async Task<List<UserDto>> GetUsersAsync(string? userName = null)
    {
        string uri = string.IsNullOrWhiteSpace(userName)
            ? "users"
            : $"users?userName={Uri.EscapeDataString(userName)}";
        HttpResponseMessage httpResponse = await client.GetAsync(uri);
        return await HttpResponseHelper.ReadAsync<List<UserDto>>(httpResponse);
    }

    public async Task DeleteUserAsync(int id)
    {
        HttpResponseMessage httpResponse = await client.DeleteAsync($"users/{id}");
        await HttpResponseHelper.EnsureSuccessAsync(httpResponse);
    }
}
