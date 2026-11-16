using System;

[Serializable]
public struct IapBmCatalog
{
    public int schemaVersion;
    public string catalogVersion;

    public IapBmCategory[] categories;
    public IapBmProduct[] products;
}

[Serializable]
public struct IapBmCategory
{
    public string categoryId;
    public string displayName;
    public int sortOrder;
}

[Serializable]
public struct IapBmProduct
{
    public string productId;
    public string categoryId;
    public string productType;
    public bool enabled;

    // 상품 또는 패키지 전체 가격
    public IapBmPrice price;

    // 플랫폼별 스토어 상품 ID
    public IapBmStoreProducts storeProducts;

    public IapBmDisplay display;

    // 구매 시 지급되는 여러 보상
    public IapBmReward[] rewards;

    public IapBmPurchaseLimit purchaseLimit;
    public IapBmAvailability availability;
}

[Serializable]
public struct IapBmPrice
{
    // KRW, USD 등
    public string currencyCode;

    // 패키지 전체 가격
    public long amount;
}

[Serializable]
public struct IapBmStoreProducts
{
    public string android;
    public string ios;
    public string windows;
}

[Serializable]
public struct IapBmDisplay
{
    public string name;
    public string description;
    public string iconAddress;
    public string badge;
    public int sortOrder;
}

[Serializable]
public struct IapBmReward
{
    public string rewardType;
    public string rewardId;
    public long quantity;
}

[Serializable]
public struct IapBmPurchaseLimit
{
    public bool enabled;
    public int maxCount;
    public string period;
}

[Serializable]
public struct IapBmAvailability
{
    public string startAtUtc;
    public string endAtUtc;
    public string[] platforms;
}