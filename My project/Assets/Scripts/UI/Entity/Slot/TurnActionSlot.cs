
using DG.Tweening;
using UnityEngine;
using UnityEngine.UIElements;

public class TurnActionSlot : BaseSlot
{
    [Header("UI ¿¬°á")]
    public Image         _Image;        //Icon Image
    public CardAction     CardData { get; private set; }


    public void SetData(CardAction data, Vector2 Position, bool bIsAnimPlay)
    {
        CardData = data;
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
}
