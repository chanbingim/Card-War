using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;
using TurnCardGame.Data;

public class InventoryController : UIBase
{
    [SerializeField] private int MaxSlotCount = 10;
    private List<InventorySlot>     _Slots;

    private TemplateContainer   _View = null;
    private VisualElement       _inventoryRoot = null;
    private VisualElement       _itemInfoPanel = null;
    private VisualElement       _itemListPanel = null;

    private void PlayAnim()
    {
        _inventoryRoot.RemoveFromClassList("popup-show");

        _inventoryRoot.AddToClassList("popup");

        _inventoryRoot.schedule.Execute(() =>
        {
            _inventoryRoot.AddToClassList("popup-show");

        }).StartingIn(10);
    }

    #region Default
    public static InventoryController Create(VisualElement _Layer)
    {
        InventoryController instance = new InventoryController();
        if(!instance.Initialize(_Layer))
        {
            instance = null;
            return null;
        }

        return instance;
    }

    bool Initialize(VisualElement _Layer)
    {
        VisualTreeAsset asset = AddressableManager.instance.Get<VisualTreeAsset>("UI/Inventory");
        if (asset == null)
            return false;

        _View = asset.Instantiate();
        if (_View == null)
            return false;

        Utility.FullScreen(_View);
        _Layer.Add(_View);

        _inventoryRoot = _View.Q<VisualElement>("Root");
        _itemInfoPanel = _View.Q<VisualElement>("ItemInfo");
        _itemListPanel = _View.Q<VisualElement>("ItemListScrollView");

        InitialzeSlot();
        return true;
    }

    private bool InitialzeSlot()
    {
        var PlayerData = GameClientManager.instance.playerData;
        int CollectionCount = PlayerData.Collections.Count;

        _Slots = new List<InventorySlot>(MaxSlotCount);
        foreach (var card in PlayerData.Collections)
        {
            ADD_Slot(card.Value);
        }
      
        return true;
    }

    private void ADD_Slot(int CardID)
    {
        var slot = InventorySlot.Create(_itemListPanel);
        slot.SetData(DataManager.instance.GetCardById(CardID));

        _Slots.Add(slot);
    }

    public override void Open()
    {
        _inventoryRoot.RemoveFromClassList("popup-Open");
        _inventoryRoot.schedule.Execute(() =>
        {
            _inventoryRoot.AddToClassList("popup-Open");

        }).StartingIn(10);
        PlayAnim();
    }

    public override void Close()
    {
        _inventoryRoot.RemoveFromClassList("popup-Close");
        _inventoryRoot.schedule.Execute(() =>
        {
            _inventoryRoot.AddToClassList("popup-Close");

        }).StartingIn(10);
    }
    #endregion
}
