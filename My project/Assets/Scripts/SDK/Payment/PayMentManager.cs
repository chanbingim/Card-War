using System;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using System.Text;

public class PayMentManager : MonoBehaviour
{
    public StringBuilder StoreName = new StringBuilder("My project (3)");
    [SerializeField] bool m_UseNative = true;
    [SerializeField] bool m_UseD2C = true;

    static public PayMentManager Instance => instance;
    static PayMentManager instance = null;

    PurchasingController m_Controller;
    IPaymentOptionProvider m_PurchaseOptions;
    readonly IRemoteCatalogProvider m_Catalog = new RemoteCatalogProvider();

    private async UniTask<bool> Initialize()
    {
        // 1. Initialize Unity Gaming Services. Skip steps 1 to 3 if your project
        //    already does this.
        await UnityServices.InitializeAsync();

        // 2. Attach the Authentication handlers before you sign the player in, so
        //    that no event is missed.
        AuthenticationService.Instance.SignedIn += OnSignedIn;
        AuthenticationService.Instance.SignInFailed += OnSignInFailed;
        AuthenticationService.Instance.SignedOut += OnSignedOut;

        // 3. Sign in. The Remote Catalog and IAP D2C Capabilities need a signed-in player.
        //    Replace anonymous sign-in with your own authentication: an anonymous player loses
        //    their purchase history when the app is reinstalled or its data is cleared.
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        // 4. Create one PurchasingController for every store you sell through.
        var storeNames = new List<string>();
        if (m_UseNative)
        {
            storeNames.Add(UnityIAPServices.GetDefaultStore());
        }
        if (m_UseD2C)
        {
            storeNames.Add(PaymentProvider.Name);
        }
        if (storeNames.Count == 0)
        {
            Debug.LogWarning("Enable at least one store.");
            return false;
        }
        m_Controller = new PurchasingController(storeNames.ToArray());

        // Subscribe to every event before you call Connect(), or you lose the events
        // that fire first. One subscription covers every store on the controller.
        m_Controller.OnStoreDisconnected += OnStoreDisconnected;
        m_Controller.OnProductsFetched += OnProductsFetched;
        m_Controller.OnProductsFetchFailed += OnProductsFetchFailed;
        m_Controller.OnPurchasePending += OnPurchasePending;
        m_Controller.OnPurchasesFetched += OnPurchasesFetched;
        m_Controller.FetchPurchases(PaymentProvider.Name);

        // Connects every store on the controller.
        await m_Controller.Connect();
        m_PurchaseOptions = m_Controller.PaymentProvidersExtendedService?.GetPaymentOptionProviderUGUI();

        // 5. One Remote Catalog works for every store.
        var result = await m_Catalog.FetchRemoteCatalog();
        if (!result.Success)
        {
            Debug.LogError($"Could not fetch the Remote Catalog: {result.Exception?.Message}");
            return false;
        }

        // Each store fetches its own products, because each store has its own product IDs.
        foreach (var storeName in m_Controller.StoreNames)
        {
            m_Controller.FetchProductsWithNoRetries(m_Catalog.GetProducts(storeName), storeName);
        }

        Debug.Log("Init");
        return true;
    }

    public void FetchPurchases()
    {
        m_Controller.FetchPurchases(PaymentProvider.Name);
    }

    // Call this from your buy button.
    public async void Buy(string catalogListingId)
    {
        if (m_PurchaseOptions != null)
        {
            // This offers every store that has this listing: native, D2C,
            // and webshop. With a single option, it buys directly without
            // showing the picker.
            await m_PurchaseOptions.ShowPurchaseOption(catalogListingId);
            return;
        }

        // No D2C store, so buy from the native store.
        if (m_Controller != null && m_UseNative)
        {
            m_Controller.PurchaseProduct(catalogListingId, UnityIAPServices.GetDefaultStore());
        }
        else
        {
            Debug.LogWarning($"No way to pay for {catalogListingId}.");
        }
    }

    // Every store's purchases arrive here, with the name of the store the player paid through.
    void OnPurchasePending(string storeName, PendingOrder order)
    {
        // Grant the content for the order here, then confirm it.
        // ConfirmPurchase sends the order back to its own store.
        Debug.Log("Item Buy");
        m_Controller.ConfirmPurchase(order);
    }

    private void OnPurchasesFetched(string storeName, Orders orders)
    {
        Debug.Log($"Pending: {orders.PendingOrders.Count}");
        Debug.Log($"Confirmed: {orders.ConfirmedOrders.Count}");
        Debug.Log($"Deferred: {orders.DeferredOrders.Count}");

        foreach (var order in orders.PendingOrders)
        {
            Debug.Log($"미완료 주문: {order}");
        }
    }

    void OnProductsFetched(string storeName, List<Product> products) =>
        Debug.Log($"{storeName}: {products.Count} products are ready to display.");

    void OnProductsFetchFailed(string storeName, ProductFetchFailed failure) =>
        Debug.LogError($"{storeName}: product fetch failed: {failure.FailureReason}");

    void OnStoreDisconnected(string storeName, StoreConnectionFailureDescription description) =>
        Debug.LogError($"{storeName}: store disconnected: {description.message}");

    void OnSignedIn() => Debug.Log($"Signed in as {AuthenticationService.Instance.PlayerId}.");
    void OnSignInFailed(RequestFailedException e) => Debug.LogError($"Sign-in failed: {e.Message}");
    void OnSignedOut() => Debug.Log("Signed out.");

    void OnDestroy()
    {
        if (m_Controller != null)
        {
            m_Controller.OnStoreDisconnected -= OnStoreDisconnected;
            m_Controller.OnProductsFetched -= OnProductsFetched;
            m_Controller.OnProductsFetchFailed -= OnProductsFetchFailed;
            m_Controller.OnPurchasePending -= OnPurchasePending;
        }

        AuthenticationService.Instance.SignedIn -= OnSignedIn;
        AuthenticationService.Instance.SignInFailed -= OnSignInFailed;
        AuthenticationService.Instance.SignedOut -= OnSignedOut;

        if (instance != null)
            instance = null;
    }

    async private void Start()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }

        instance = this;
        await instance.Initialize();
        DontDestroyOnLoad(instance.gameObject);
    }
}
