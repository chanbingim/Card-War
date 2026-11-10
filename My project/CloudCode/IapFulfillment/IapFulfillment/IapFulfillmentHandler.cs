using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Unity.Services.CloudCode.Apis;
using Unity.Services.CloudCode.Core;
using Unity.Services.CloudCode.Shared;
using Unity.Services.CloudSave.Model;

namespace IapFulfillment;

public class IapFulfillmentHandler
{
    private const string InventoryKey = "player_inventory";
    private const string IapApiBaseUrl = "https://iap.services.api.unity.com/v1";

    private static readonly HttpClient HttpClient = new()
    {
        Timeout = TimeSpan.FromSeconds(10)
    };

    private readonly ILogger<IapFulfillmentHandler> _logger;

    public IapFulfillmentHandler(ILogger<IapFulfillmentHandler> logger)
    {
        _logger = logger;
    }

    [CloudCodeFunction("ProcessPurchaseFulfillment")]
    public async Task<WebhookResponse> ProcessPurchaseFulfillment(
        IExecutionContext context,
        IGameApiClient gameApiClient,
        Guid id,
        string version,
        string eventType,
        DateTime time,
        string projectId,
        string environmentId,
        string dataType,
        WebhookOrderData data)
    {
        if (!string.Equals(eventType, "order.paid", StringComparison.Ordinal))
        {
            _logger.LogInformation(
                "Ignoring IAP event {EventType} for order {OrderId}",
                eventType,
                data.Id);
            return WebhookResponse.Ok();
        }

        try
        {
            ValidateRequest(context, projectId, environmentId, data);

            bool granted = await GrantItemsOnceAsync(context, gameApiClient, data);
            await MarkOrderFulfilledAsync(
                context.ServiceToken!,
                projectId,
                environmentId,
                data.Id);

            _logger.LogInformation(
                "IAP order {OrderId} processed. Newly granted={NewlyGranted}",
                data.Id,
                granted);

            return WebhookResponse.Ok();
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "IAP fulfillment failed for order {OrderId}",
                data.Id);

            // Returning an error makes Unity IAP retry this webhook.
            return WebhookResponse.Error(exception.Message);
        }
    }

    private static void ValidateRequest(
        IExecutionContext context,
        string projectId,
        string environmentId,
        WebhookOrderData order)
    {
        if (string.IsNullOrWhiteSpace(context.ServiceToken))
            throw new InvalidOperationException("Cloud Code service token is missing.");
        if (string.IsNullOrWhiteSpace(projectId) || string.IsNullOrWhiteSpace(environmentId))
            throw new InvalidOperationException("Project or environment ID is missing.");
        if (string.IsNullOrWhiteSpace(order.Id) || string.IsNullOrWhiteSpace(order.PlayerId))
            throw new InvalidOperationException("Order or player ID is missing.");
        if (order.LineItems.Count == 0)
            throw new InvalidOperationException("The paid order has no line items.");
    }

    private async Task<bool> GrantItemsOnceAsync(
        IExecutionContext context,
        IGameApiClient gameApiClient,
        WebhookOrderData order)
    {
        const int maxAttempts = 3;
        for (int attempt = 1; attempt <= maxAttempts; attempt++)
        {
            InventorySnapshot snapshot = await GetInventoryAsync(
                context,
                gameApiClient,
                order.PlayerId);
            PlayerInventory inventory = snapshot.Inventory;

            if (inventory.ProcessedOrderIds.Contains(order.Id))
            {
                _logger.LogInformation(
                    "Order {OrderId} was already granted; skipping duplicate grant",
                    order.Id);
                return false;
            }

            foreach (WebhookLineItem lineItem in order.LineItems)
                ApplyReward(inventory, lineItem.Sku);

            // Save the order ID with the reward. If the Orders API call fails afterwards,
            // a webhook retry skips the reward and retries only the fulfilled update.
            inventory.ProcessedOrderIds.Add(order.Id);
            inventory.LastProcessedOrderId = order.Id;
            inventory.UpdatedAt = DateTime.UtcNow;

            try
            {
                await SaveInventoryAsync(
                    context,
                    gameApiClient,
                    order.PlayerId,
                    inventory,
                    snapshot.WriteLock);
                return true;
            }
            catch (ApiException exception) when (
                exception.Response.StatusCode == HttpStatusCode.Conflict &&
                attempt < maxAttempts)
            {
                _logger.LogWarning(
                    "Cloud Save write conflict for order {OrderId}; retrying {Attempt}/{MaxAttempts}",
                    order.Id,
                    attempt,
                    maxAttempts);
            }
        }

        throw new InvalidOperationException(
            $"Could not grant order {order.Id} after {maxAttempts} write conflicts.");
    }

    private static void ApplyReward(PlayerInventory inventory, string sku)
    {
        switch (sku)
        {
            // The current mock catalog item shown in this project.
            case "Test":
            case "test":
            case "coins_100":
                inventory.Cash += 100;
                break;

            case "coins_500":
                inventory.Cash += 500;
                break;

            default:
                // Do not fulfill a paid SKU until its server reward is configured.
                throw new InvalidOperationException(
                    $"No server reward is configured for SKU '{sku}'.");
        }
    }

    private async Task<InventorySnapshot> GetInventoryAsync(
        IExecutionContext context,
        IGameApiClient gameApiClient,
        string playerId)
    {
        var response = await gameApiClient.CloudSaveData.GetProtectedItemsAsync(
            context,
            context.ServiceToken!,
            context.ProjectId!,
            playerId,
            new List<string> { InventoryKey });

        if (response.Data.Results.Count == 0 || response.Data.Results[0].Value == null)
            return new InventorySnapshot(new PlayerInventory(), null);

        string? json = response.Data.Results[0].Value.ToString();
        if (string.IsNullOrWhiteSpace(json))
            return new InventorySnapshot(new PlayerInventory(), response.Data.Results[0].WriteLock);

        PlayerInventory inventory =
            JsonSerializer.Deserialize<PlayerInventory>(json) ?? new PlayerInventory();
        return new InventorySnapshot(inventory, response.Data.Results[0].WriteLock);
    }

    private static async Task SaveInventoryAsync(
        IExecutionContext context,
        IGameApiClient gameApiClient,
        string playerId,
        PlayerInventory inventory,
        string? writeLock)
    {
        string json = JsonSerializer.Serialize(inventory);
        SetItemBody body = writeLock == null
            ? new SetItemBody(InventoryKey, json)
            : new SetItemBody(InventoryKey, json, writeLock);

        await gameApiClient.CloudSaveData.SetProtectedItemAsync(
            context,
            context.ServiceToken!,
            context.ProjectId!,
            playerId,
            body);
    }

    private async Task MarkOrderFulfilledAsync(
        string serviceToken,
        string projectId,
        string environmentId,
        string orderId)
    {
        string url = $"{IapApiBaseUrl}/projects/{projectId}/environments/" +
                     $"{environmentId}/orders/{orderId}";

        using var patch = new HttpRequestMessage(HttpMethod.Patch, url)
        {
            Content = new StringContent(
                "{\"status\":\"fulfilled\"}",
                Encoding.UTF8,
                "application/json")
        };
        patch.Headers.Authorization = new AuthenticationHeaderValue("Bearer", serviceToken);

        using HttpResponseMessage response = await HttpClient.SendAsync(patch);
        if (response.IsSuccessStatusCode)
            return;

        // The first call can succeed while its webhook response is lost. Verify a
        // conflict before treating a retry as a failure.
        if (response.StatusCode == HttpStatusCode.Conflict &&
            await IsOrderAlreadyFulfilledAsync(url, serviceToken))
        {
            return;
        }

        string body = await response.Content.ReadAsStringAsync();
        throw new HttpRequestException(
            $"Orders API PATCH failed ({(int)response.StatusCode}): {body}");
    }

    private static async Task<bool> IsOrderAlreadyFulfilledAsync(
        string url,
        string serviceToken)
    {
        using var get = new HttpRequestMessage(HttpMethod.Get, url);
        get.Headers.Authorization = new AuthenticationHeaderValue("Bearer", serviceToken);

        using HttpResponseMessage response = await HttpClient.SendAsync(get);
        if (!response.IsSuccessStatusCode)
            return false;

        string json = await response.Content.ReadAsStringAsync();
        using JsonDocument document = JsonDocument.Parse(json);
        return document.RootElement.TryGetProperty("status", out JsonElement status) &&
               string.Equals(status.GetString(), "fulfilled", StringComparison.OrdinalIgnoreCase);
    }
}

public sealed record InventorySnapshot(PlayerInventory Inventory, string? WriteLock);

public sealed class PlayerInventory
{
    public int Cash { get; set; }
    public int Gold { get; set; }
    public HashSet<string> ProcessedOrderIds { get; set; } = new(StringComparer.Ordinal);
    public string? LastProcessedOrderId { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public sealed class WebhookOrderData
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("playerId")]
    public string PlayerId { get; set; } = string.Empty;

    [JsonPropertyName("paymentProvider")]
    public string PaymentProvider { get; set; } = string.Empty;

    [JsonPropertyName("paymentProviderResourceId")]
    public string? PaymentProviderResourceId { get; set; }

    [JsonPropertyName("lineItems")]
    public List<WebhookLineItem> LineItems { get; set; } = new();

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;
}

public sealed class WebhookLineItem
{
    [JsonPropertyName("sku")]
    public string Sku { get; set; } = string.Empty;

    [JsonPropertyName("productType")]
    public string ProductType { get; set; } = string.Empty;
}

public sealed class WebhookResponse
{
    public string Status { get; set; } = string.Empty;
    public string? Code { get; set; }
    public string? Description { get; set; }

    public static WebhookResponse Ok() => new()
    {
        Status = "ok"
    };

    public static WebhookResponse Error(string description) => new()
    {
        Status = "error",
        Code = "declined",
        Description = description
    };
}
