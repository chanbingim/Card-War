using System;

interface IPaymentProvider
{
   event Action<string> OnPurchaseSuccess;
   event Action<string> OnPurchaseFailed;

   void Initialize();
   void Buy(string productId);
}
