using System;

public class PaymentAndroid : IPaymentProvider
{
    public event Action<string> OnPurchaseSuccess;
    public event Action<string> OnPurchaseFailed;

    public void Initialize()
    {

    }

    public void Buy(string ItemName)
    {

    }
}
