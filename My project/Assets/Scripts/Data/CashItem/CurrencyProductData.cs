using System;

[Serializable]
public struct CurrencyProduct
{
    public string productId;
    public string productType;
    public bool enabled;

    public string displayName;
    public string description;
    public string icon;
    public int sortOrder;

    public string currency;
    public int price;

    public CurrencyProductGrant[] grants;

    // 0이면 구매 제한 없음
    public int purchaseLimit;
}

[Serializable]
public struct CurrencyProductGrant
{
    public string   itemId;
    public int      quantity;
}

[Serializable]
public struct CurrencyProductCatalog
{
    public int schemaVersion;
    public string catalogVersion;
    public CurrencyProduct[] products;
}

[Serializable]
public struct BMReward
{
    public string Type;
    public string ItemId;
    public int Amount;
}