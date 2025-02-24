namespace IPLocate.Workflow;

using System.Text.Json;
using Microsoft.Extensions.Logging;
using Temporalio.Activities;

public class Activities
{
    private readonly HttpClient client;

    public Activities(HttpClient client) => this.client = client;

    [Activity]
    public async Task<string> GetIPAsync()
    {
        char[] ignoreChars = { '\n' };
        var response = await client.GetAsync("https://icanhazip.com");
        var content = await response.Content.ReadAsStringAsync();
        response.EnsureSuccessStatusCode();
        return content.Trim(ignoreChars);
    }

    [Activity]
    public async Task<string> GetLocationInfoAsync(string ip)
    {
        var response = await client.GetAsync($"http://ip-api.com/json/{ip}");
        var content = await response.Content.ReadAsStringAsync();
        response.EnsureSuccessStatusCode();
        var jsonResponse = JsonSerializer.Deserialize<JsonElement>(content);
        var city = jsonResponse.GetProperty("city").GetString() ?? string.Empty;
        var regionName = jsonResponse.GetProperty("regionName").GetString() ?? string.Empty;
        var country = jsonResponse.GetProperty("country").GetString() ?? string.Empty;
        return $"{city}, {regionName}, {country}";
    }
}