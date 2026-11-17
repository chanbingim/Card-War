using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using Unity.Services.CloudCode.Core;

namespace IapFulfillment;

/// <summary>Reads authoritative order details from the Unity IAP Orders API.</summary>
public sealed class UnityIapOrderClient
{
    private const string ApiBaseUrl = "https://iap.services.api.unity.com/v1";
    private static readonly HttpClient HttpClient = new() { Timeout = TimeSpan.FromSeconds(10) };

    public async Task<IapOrder> GetOrderAsync(IExecutionContext context, string orderId)
    {
        string url = $"{ApiBaseUrl}/projects/{context.ProjectId}" +
                     $"/environments/{context.EnvironmentId}/orders/{Uri.EscapeDataString(orderId)}";
        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.ServiceToken);

        using HttpResponseMessage response = await HttpClient.SendAsync(request);
        string body = await response.Content.ReadAsStringAsync();
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            throw new InvalidOperationException("The order does not exist in Unity IAP.");
        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException($"Unity IAP order lookup failed: {(int)response.StatusCode} {body}");

        return JsonSerializer.Deserialize<IapOrder>(
                   body,
                   new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
               ?? throw new InvalidOperationException("Could not parse the Unity IAP order response.");
    }
}

public sealed class IapOrder
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("playerId")]
    public string PlayerId { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("lineItems")]
    public List<IapOrderItem> LineItems { get; set; } = new();
}

public sealed class IapOrderItem
{
    [JsonPropertyName("sku")]
    public string Sku { get; set; } = string.Empty;

    [JsonPropertyName("productType")]
    public string ProductType { get; set; } = string.Empty;
}