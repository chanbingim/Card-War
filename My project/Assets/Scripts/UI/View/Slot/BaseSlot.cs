using System;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class BaseSlot : UIBase
{
    protected int             _SlotID;

    public event Action<BaseSlot, BaseSlot> OnSwap;
    public event Action<BaseSlot>           OnHoverEnter;
    public event Action<BaseSlot>           OnHoverExit;
    public event Action<BaseSlot>           OnDrop;
    public event Action<BaseSlot>           OnChangedItem;

    protected void ChangedItem()
    {
        OnChangedItem?.Invoke(this);
    }

    protected virtual void HoverEnter()
    {
        OnHoverEnter?.Invoke(this);
    }

    protected virtual void HoverExit()
    {
        OnHoverExit?.Invoke(this);
    }

    protected virtual void Drop()
    {
        OnDrop?.Invoke(this);
    }

    protected virtual void Swap(BaseSlot target)
    {
        OnSwap?.Invoke(this, target);
    }

    #region Default

    #endregion
}
