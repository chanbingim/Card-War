using Microsoft.Extensions.Logging;
using Unity.Services.CloudCode.Core;

namespace IapFulfillment;

/// <summary>Receives purchase-validation requests and coordinates order and reward data.</summary>
public sealed class IapFulfillmentHandler
{
    private static readonly UnityIapOrderClient OrderClient = new();
    private static readonly RewardCatalogProvider RewardCatalog = new();
    private readonly ILogger<IapFulfillmentHandler> _logger;

    public IapFulfillmentHandler(ILogger<IapFulfillmentHandler> logger)
    {
        _logger = logger;
    }

    [CloudCodeFunction("ProcessPurchase")]
    public async Task<PurchaseResult> ProcessPurchase(
        IExecutionContext context,
        string orderId)
    {
        try
        {
            ValidateRequest(context, orderId);
            IapOrder order = await OrderClient.GetOrderAsync(context, orderId);

            if (!string.Equals(order.Id, orderId, StringComparison.Ordinal))
                throw new InvalidOperationException("Unity IAP returned a different order ID.");
            if (!string.Equals(order.PlayerId, context.PlayerId, StringComparison.Ordinal))
                throw new InvalidOperationException("The order does not belong to the authenticated player.");
            if (!string.Equals(order.Status, "paid", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException($"Order is not paid: {order.Status}");
            if (order.LineItems.Count == 0)
                throw new InvalidOperationException("The order has no line items.");

            var rewards = new List<GrantedReward>();
            foreach (IapOrderItem lineItem in order.LineItems)
                rewards.AddRange(RewardCatalog.GetRewardsForSku(lineItem.Sku));

            return new PurchaseResult
            {
                Success = true,
                OrderId = order.Id,
                Message = "Order verified; reward data returned. This endpoint does not modify inventory.",
                GrantedRewards = rewards
            };
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Purchase validation failed. OrderId={OrderId}", orderId);
            return new PurchaseResult
            {
                Success = false,
                OrderId = orderId,
                Message = exception.Message
            };
        }
    }

    private static void ValidateRequest(IExecutionContext context, string orderId)
    {
        if (string.IsNullOrWhiteSpace(context.PlayerId))
            throw new InvalidOperationException("Authenticated Player ID is missing.");
        if (string.IsNullOrWhiteSpace(context.ProjectId))
            throw new InvalidOperationException("Project ID is missing.");
        if (string.IsNullOrWhiteSpace(context.EnvironmentId))
            throw new InvalidOperationException("Environment ID is missing.");
        if (string.IsNullOrWhiteSpace(context.ServiceToken))
            throw new InvalidOperationException("Service token is missing.");
        if (string.IsNullOrWhiteSpace(orderId))
            throw new ArgumentException("Order ID is empty.", nameof(orderId));
    }
}

public sealed class PurchaseResult
{
    public bool Success { get; set; }
    public string OrderId { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public List<GrantedReward> GrantedRewards { get; set; } = new();
}