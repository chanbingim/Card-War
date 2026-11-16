using System.Net.Http.Headers;
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

    private const string IapApiBaseUrl =
        "https://iap.services.api.unity.com/v1";

    // Remote Catalog의 SKU와 실제 지급 내용을 서버에서 관리합니다.
    // 아래 SKU는 샘플이므로 프로젝트의 실제 SKU로 교체하세요.
    private static readonly IReadOnlyDictionary<string, RewardDefinition[]> RewardsBySku =
        new Dictionary<string, RewardDefinition[]>(StringComparer.Ordinal)
        {
            ["Test3"] =
            [
                new RewardDefinition(RewardType.Gold, string.Empty, 100)
            ],
            ["Test33"] =
            [
                new RewardDefinition(RewardType.Gem, string.Empty, 10)
            ],
            ["Gold"] =
            [
                new RewardDefinition(RewardType.Gold, string.Empty, 500),
                new RewardDefinition(RewardType.Gem, string.Empty, 50),
                new RewardDefinition(RewardType.Item, "starter_sword", 1)
            ]
        };

    private static readonly HttpClient HttpClient = new()
    {
        Timeout = TimeSpan.FromSeconds(10)
    };

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly ILogger<IapFulfillmentHandler> _logger;

    public IapFulfillmentHandler(ILogger<IapFulfillmentHandler> logger)
    {
        _logger = logger;
    }

    [CloudCodeFunction("Ping")]
    public Task<string> Ping(IExecutionContext context)
    {
        _logger.LogInformation(
            "Ping called. PlayerId={PlayerId}",
            context.PlayerId);

        return Task.FromResult("pong");
    }

    [CloudCodeFunction("ProcessPurchase")]
    public async Task<PurchaseResult> ProcessPurchase(
        IExecutionContext context,
        IGameApiClient gameApiClient,
        string orderId)
    {
        try
        {
            ValidateRequest(context, orderId);

            // orderId로 조회되면 Unity IAP에 등록된 주문입니다.
            IapOrder order = await GetOrderFromUnityIap(context, orderId);

            if (!string.Equals(order.Id, orderId, StringComparison.Ordinal))
            {
                throw new InvalidOperationException(
                    "조회된 주문 ID가 요청한 주문 ID와 일치하지 않습니다.");
            }

            if (!string.Equals(
                    order.PlayerId,
                    context.PlayerId,
                    StringComparison.Ordinal))
            {
                throw new InvalidOperationException(
                    "주문 소유자가 일치하지 않습니다.");
            }

            if (!string.Equals(
                    order.Status,
                    "paid",
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    $"결제가 완료되지 않은 주문입니다: {order.Status}");
            }

            if (order.LineItems.Count == 0)
                throw new InvalidOperationException("주문에 상품이 없습니다.");

            List<GrantedReward> grantedRewards = DescribeRewards(order.LineItems);

            foreach (IapOrderItem lineItem in order.LineItems)
            {
                GrantRewardsForSku(
                    inventory,
                    lineItem.Sku);
            }

            return new PurchaseResult
            {
                Success = true,
                AlreadyProcessed = false,
                OrderId = order.Id,
                Message = "결제 검증과 상품 지급이 완료되었습니다.",
                GrantedRewards = grantedRewards
            };
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Purchase validation failed. OrderId={OrderId}",
                orderId);

            return new PurchaseResult
            {
                Success = false,
                OrderId = orderId,
                Message = exception.Message
            };
        }
    }

    private static void ValidateRequest(
        IExecutionContext context,
        string orderId)
    {
        if (string.IsNullOrWhiteSpace(context.PlayerId))
            throw new InvalidOperationException("인증된 Player ID가 없습니다.");

        if (string.IsNullOrWhiteSpace(context.ProjectId))
            throw new InvalidOperationException("Project ID가 없습니다.");

        if (string.IsNullOrWhiteSpace(context.EnvironmentId))
            throw new InvalidOperationException("Environment ID가 없습니다.");

        if (string.IsNullOrWhiteSpace(context.ServiceToken))
            throw new InvalidOperationException("Service Token이 없습니다.");

        if (string.IsNullOrWhiteSpace(orderId))
            throw new ArgumentException("Order ID가 비어 있습니다.", nameof(orderId));
    }

    private static async Task<IapOrder> GetOrderFromUnityIap(
        IExecutionContext context,
        string orderId)
    {
        string url =
            $"{IapApiBaseUrl}/projects/{context.ProjectId}" +
            $"/environments/{context.EnvironmentId}" +
            $"/orders/{Uri.EscapeDataString(orderId)}";

        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            context.ServiceToken);

        using HttpResponseMessage response =
            await HttpClient.SendAsync(request);

        string responseBody =
            await response.Content.ReadAsStringAsync();

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            throw new InvalidOperationException("Unity IAP에 존재하지 않는 주문입니다.");

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException(
                $"Unity IAP 주문 조회 실패: " +
                $"{(int)response.StatusCode} {responseBody}");
        }

        IapOrder? order = JsonSerializer.Deserialize<IapOrder>(
            responseBody,
            JsonOptions);

        return order ?? throw new InvalidOperationException(
            "Unity IAP 주문 응답을 변환하지 못했습니다.");
    }

    private static void GrantRewardsForSku(
        PlayerInventory inventory,
        string sku)
    {
        if (!RewardsBySku.TryGetValue(sku, out RewardDefinition[]? rewards))
            throw new InvalidOperationException($"서버에 등록되지 않은 SKU입니다: {sku}");

        foreach (RewardDefinition reward in rewards)
        {
            switch (reward.Type)
            {
                case RewardType.Gold:
                    inventory.Gold += reward.Amount;
                    break;

                case RewardType.Gem:
                    inventory.Gems += reward.Amount;
                    break;

                case RewardType.Item:
                    inventory.Items.TryGetValue(reward.ItemId, out int currentAmount);
                    inventory.Items[reward.ItemId] = currentAmount + reward.Amount;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(reward.Type),
                        reward.Type,
                        "지원하지 않는 보상 종류입니다.");
            }

        }
    }

    private static List<GrantedReward> DescribeRewards(
        IEnumerable<IapOrderItem> lineItems)
    {
        var result = new List<GrantedReward>();

        foreach (IapOrderItem lineItem in lineItems)
        {
            if (!RewardsBySku.TryGetValue(
                    lineItem.Sku,
                    out RewardDefinition[]? rewards))
            {
                throw new InvalidOperationException(
                    $"서버에 등록되지 않은 SKU입니다: {lineItem.Sku}");
            }

            foreach (RewardDefinition reward in rewards)
            {
                result.Add(new GrantedReward
                {
                    Type = reward.Type.ToString(),
                    ItemId = reward.ItemId,
                    Amount = reward.Amount
                });
            }
        }

        return result;
    }
}

public class PurchaseResult
{
    public bool Success { get; set; }
    public bool AlreadyProcessed { get; set; }
    public string OrderId { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public List<GrantedReward> GrantedRewards { get; set; } = new();
}

public class IapOrder
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

public class IapOrderItem
{
    [JsonPropertyName("sku")]
    public string Sku { get; set; } = string.Empty;

    [JsonPropertyName("productType")]
    public string ProductType { get; set; } = string.Empty;
}

public enum RewardType
{
    Gold,
    Gem,
    Item
}

public sealed record RewardDefinition(
    RewardType Type,
    string ItemId,
    int Amount);

public class GrantedReward
{
    public string Type { get; set; } = string.Empty;
    public string ItemId { get; set; } = string.Empty;
    public int Amount { get; set; }
}

public class PlayerInventory
{
    public int Gold { get; set; }
    public int Gems { get; set; }
    public Dictionary<string, int> Items { get; set; } = new();
    public HashSet<string> ProcessedOrderIds { get; set; } = new();
}
