using System.Security.Claims;
using System.Text.Json;
using ApiContracts;
using BlazorApp.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace BlazorApp.Auth;

public class SimpleAuthProvider : AuthenticationStateProvider
{
    private readonly HttpClient httpClient;
    private readonly IJSRuntime jsRuntime;

    // "Primary cache" so we don't hit sessionStorage every time
    private ClaimsPrincipal? cachedPrincipal;

    public SimpleAuthProvider(HttpClient httpClient, IJSRuntime jsRuntime)
    {
        this.httpClient = httpClient;
        this.jsRuntime = jsRuntime;
    }

    public async Task LoginAsync(string userName, string password)
    {
        HttpResponseMessage response = await httpClient.PostAsJsonAsync(
            "auth/login",
            new LoginRequest(userName, password));

        string content = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(HttpResponseHelper.ExtractError(content));
        }

        UserDto userDto = JsonSerializer.Deserialize<UserDto>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })!;

        // Cache the user in the browser, so a refresh doesn't log you out
        string serialisedData = JsonSerializer.Serialize(userDto);
        await jsRuntime.InvokeVoidAsync("sessionStorage.setItem", "currentUser", serialisedData);

        cachedPrincipal = CreatePrincipal(userDto);
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(cachedPrincipal)));
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        if (cachedPrincipal is not null)
        {
            return new AuthenticationState(cachedPrincipal);
        }

        string? userAsJson;
        try
        {
            userAsJson = await jsRuntime.InvokeAsync<string?>("sessionStorage.getItem", "currentUser");
        }
        catch (InvalidOperationException)
        {
            // JS is not available yet (e.g. during prerendering)
            return new AuthenticationState(new());
        }

        if (string.IsNullOrEmpty(userAsJson))
        {
            return new AuthenticationState(new());
        }

        UserDto userDto = JsonSerializer.Deserialize<UserDto>(userAsJson)!;
        cachedPrincipal = CreatePrincipal(userDto);
        return new AuthenticationState(cachedPrincipal);
    }

    public async Task LogoutAsync()
    {
        cachedPrincipal = null;
        await jsRuntime.InvokeVoidAsync("sessionStorage.setItem", "currentUser", "");
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(new())));
    }

    private static ClaimsPrincipal CreatePrincipal(UserDto userDto)
    {
        List<Claim> claims = new()
        {
            new Claim(ClaimTypes.Name, userDto.UserName),
            new Claim("Id", userDto.Id.ToString())
        };
        ClaimsIdentity identity = new(claims, "apiauth");
        return new ClaimsPrincipal(identity);
    }
}
