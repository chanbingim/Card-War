
using DG.Tweening;
using Spine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class TurnActionSlot : BaseSlot
{
    [Header("UI ¿¬°á")]
    public float        _DurAnimtime;

    [SerializeField] private Image          _Image;             //Icon Image
    [SerializeField] private Image          _ActionIcon;        //Battle Image
    [SerializeField] private RectTransform  _RectTransform;

    private CharacterAction _Data = null;

    public void Awake()
    {

    }

    public void SetData(CharacterAction data, Vector2 Position, bool bIsAnimPlay)
    {
        _Data = data;

        try
        {
            var DataMgr = DataManager.instance;
            if (DataMgr == null)
            {
                throw new System.Exception("[Trun Action Slot] Not Find DataManager");
            }

            if (data.ActType == EACTION_TYPE.USE_CARD)
            {
                var Data = _Data as CardAction;
              
                _Image.sprite = DataMgr.GetCardSprite(Data.CardData.CardID);
                if (bIsAnimPlay)
                {
                    _RectTransform.DOKill();
                    _RectTransform.DOAnchorPos(Position, _DurAnimtime);
                }
            }
            else if (data.ActType == EACTION_TYPE.ATTACK)
            {
                var Data = _Data as BattleAction;

                var AddressableMgr = AddressableManager.instance;
                if (AddressableMgr == null)
                {
                    throw new System.Exception("[Trun Action Slot] Not Find Addressable");
                }

                var AssetRef = Data.ActObject.Data.Source.CharacterIcon;
                var atlas = AddressableMgr.Get<SpriteAtlas>(GAME_CONST.Const.CharacterIconAddress);
               
                _Image.sprite = atlas.GetSprite(AssetRef.SubObjectName);
                _ActionIcon.gameObject.SetActive(true);
                if (bIsAnimPlay)
                {
                    _RectTransform.DOKill();
                    _RectTransform.DOAnchorPos(Position, _DurAnimtime);
                }
            }
        }
        catch (System.Exception msg)
        {
            Debug.LogException(msg);
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
