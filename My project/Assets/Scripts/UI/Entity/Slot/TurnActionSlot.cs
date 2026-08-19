
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class TurnActionSlot : BaseSlot
{
    [Header("UI ¿¬°á")]
    public float        _DurAnimtime;

    [SerializeField] private Image          _Image;        //Icon Image
    [SerializeField] private RectTransform  _RectTransform;

    private CardAction _CardData = null;

    public void Awake()
    {

    }

    public void SetData(CharacterAction data, Vector2 Position, bool bIsAnimPlay)
    {
        _CardData = data as CardAction;

        _Image.sprite = DataManager.instance.GetCardSprite(_CardData.CardData.CardID);
        if(bIsAnimPlay)
        {
            _RectTransform.DOKill();
            _RectTransform.DOAnchorPos(Position, _DurAnimtime);
        }
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

    private void OnDisable()
    {
        _RectTransform.DOKill();
        transform.DOKill();
    }

    protected override void OnDestroy()
    {
        _RectTransform.DOKill();
        base.OnDestroy();
    }
}
