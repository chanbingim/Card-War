
using TurnCardGame.Data;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.UIElements;

public class InventorySlot : BaseSlot
{
    public CardData    CardData { get; private set; }

    public void SetData(CardData data)
    {
        CardData = data;
        

        Sprite sprite = DataManager.instance.GetCardSprite(CardData.SpriteID);
        VisualElement image = this.Q<VisualElement>("Icon");
        image.style.backgroundImage = new StyleBackground(sprite);
    }

    protected override void HoverEnter()
    {
        base.HoverEnter();
    }

    protected override void HoverExit()
    {
        base.HoverExit();
    }

    protected override void Drop()
    {
        base.Drop();
    }

    protected override void Swap(BaseSlot target)
    {
        base.Swap(target);
    }

    #region Default
    public static InventorySlot Create(VisualElement Layer)
    {
        InventorySlot instance = new InventorySlot();
        if(!instance.Initialize(Layer))
            instance = null;

        return instance;
    }

    private bool Initialize(VisualElement Layer)
    {
        VisualTreeAsset asset = AddressableManager.instance.Get<VisualTreeAsset>("UI/InventorySlot");
        if (asset == null)
            return false;

        asset.CloneTree(this);
        Layer.Add(this);
        return true;
    }

    #endregion
}
