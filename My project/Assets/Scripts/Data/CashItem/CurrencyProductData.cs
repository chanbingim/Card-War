using UnityEngine;
using static CurrencyComponent;

[CreateAssetMenu(menuName = "Shop/CurrencyProduct")]
public class CurrencyProductData : ScriptableObject
{
    public string ProductID;

    public CurrencyType CurrencyType;

    public int Amount;

    public int BonusAmount;

    public float Price;
}