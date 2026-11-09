using System.Text.Json;

namespace BlazorApp.Services;

// Shared code for reading responses from the Web API
internal static class HttpResponseHelper
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public static async Task<T> ReadAsync<T>(HttpResponseMessage httpResponse)
    {
        string response = await httpResponse.Content.ReadAsStringAsync();
        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception(ExtractError(response));
        }
        return JsonSerializer.Deserialize<T>(response, JsonOptions)!;
    }

    public static async Task EnsureSuccessAsync(HttpResponseMessage httpResponse)
    {
        if (!httpResponse.IsSuccessStatusCode)
        {
            string response = await httpResponse.Content.ReadAsStringAsync();
            throw new Exception(ExtractError(response));
        }
    }

    // The Web API's GlobalExceptionHandler returns { status, error }; show just the error text
    public static string ExtractError(string response)
    {
        try
        {
            using JsonDocument doc = JsonDocument.Parse(response);
            if (doc.RootElement.ValueKind == JsonValueKind.Object &&
                doc.RootElement.TryGetProperty("error", out JsonElement error))
            {
                return error.GetString() ?? response;
            }
        }
        catch (JsonException)
        {
            // not JSON, use the raw text
        }
        return string.IsNullOrWhiteSpace(response) ? "Something went wrong" : response;
    }
}
