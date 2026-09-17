using UnityEngine;
using static PlayerData;

[CreateAssetMenu(menuName = "Shop/CurrencyProduct")]
public class CurrencyProductData : ScriptableObject
{
    public string ProductID;

    public ECurrency    CurrencyType;

    public int Amount;

    public int BonusAmount;

    public float Price;
}