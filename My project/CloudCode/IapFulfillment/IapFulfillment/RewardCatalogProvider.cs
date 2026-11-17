using System.Text.Json;

namespace IapFulfillment;

/// <summary>Loads the server-owned reward catalog embedded in the Cloud Code module.</summary>
public sealed class RewardCatalogProvider
{
    private const string ResourceName = "IapFulfillment.Data.CurrencyProductGrants.json";
    private readonly Dictionary<string, List<RewardDefinition>> _rewardsBySku;

    public RewardCatalogProvider()
    {
        _rewardsBySku = Load();
    }

    public List<GrantedReward> GetRewardsForSku(string sku)
    {
        if (!_rewardsBySku.TryGetValue(sku, out List<RewardDefinition>? rewards))
            throw new InvalidOperationException($"No reward data is configured for SKU: {sku}");

        return rewards.Select(reward => new GrantedReward
        {
            Type = reward.Type.ToString(),
            ItemId = reward.ItemId,
            Amount = reward.Quantity
        }).ToList();
    }

    private static Dictionary<string, List<RewardDefinition>> Load()
    {
        using Stream stream = typeof(RewardCatalogProvider).Assembly
            .GetManifestResourceStream(ResourceName)
            ?? throw new InvalidOperationException($"Embedded reward catalog not found: {ResourceName}");

        RewardCatalogDocument catalog = JsonSerializer.Deserialize<RewardCatalogDocument>(
            stream,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
            ?? throw new InvalidOperationException("Reward catalog JSON is empty or invalid.");

        var result = new Dictionary<string, List<RewardDefinition>>(StringComparer.Ordinal);
        foreach (RewardProductData product in catalog.Products)
        {
            string sku = product.ProductId.Trim();
            if (string.IsNullOrWhiteSpace(sku))
                throw new InvalidOperationException("Reward catalog contains an empty productId.");

            var rewards = new List<RewardDefinition>();
            foreach (RewardGrantData grant in product.Grants)
            {
                string itemId = grant.ItemId.Trim();
                if (string.IsNullOrWhiteSpace(itemId) || grant.Quantity <= 0)
                    throw new InvalidOperationException($"Invalid reward entry for SKU {sku}.");

                RewardType type = itemId.ToLowerInvariant() switch
                {
                    "gold" => RewardType.Gold,
                    "gem" => RewardType.Gem,
                    _ => RewardType.Item
                };
                rewards.Add(new RewardDefinition(
                    type,
                    type == RewardType.Item ? itemId : string.Empty,
                    grant.Quantity));
            }

            if (rewards.Count == 0)
                throw new InvalidOperationException($"Reward catalog has no grants for SKU {sku}.");
            result.Add(sku, rewards);
        }

        return result;
    }
}

public sealed class RewardCatalogDocument
{
    public int SchemaVersion { get; set; }
    public string CatalogVersion { get; set; } = string.Empty;
    public List<RewardProductData> Products { get; set; } = new();
}

public sealed class RewardProductData
{
    public string ProductId { get; set; } = string.Empty;
    public List<RewardGrantData> Grants { get; set; } = new();
}

public sealed class RewardGrantData
{
    public string ItemId { get; set; } = string.Empty;
    public int Quantity { get; set; }
}

public enum RewardType
{
    Gold,
    Gem,
    Item
}

public sealed record RewardDefinition(RewardType Type, string ItemId, int Quantity);

public sealed class GrantedReward
{
    public string Type { get; set; } = string.Empty;
    public string ItemId { get; set; } = string.Empty;
    public int Amount { get; set; }
}