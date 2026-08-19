using TurnCardGame.Data;
using UnityEngine;
using static TurnManager;

public class Character : MonoBehaviour, IActionDragHandler
{
    #region Delegate
    public delegate void FinishedAction();
    public delegate void OnDagmaed(float fHealthRatio);
    public delegate void OnChangeState(CharacterRuntimeData Data);

    public event OnDagmaed OnDamaged;
    public event OnChangeState OnChangedState;
    public event FinishedAction OnFinishedAct;
    #endregion

    int ID = 0;
    public CharacterRuntimeData Data { get; private set; }

    private Material        _material = null;
    private bool            _bIsAttackAble = false;

    void Start()
    {
        SpriteRenderer spriteRender = GetComponent<SpriteRenderer>();
        if(spriteRender != null )
        {
            _material = spriteRender.material;
        }
    }

    public void Initialize(int CharacterID, Vector3 Position)
    {
        transform.position = Position;
        ID = CharacterID;
        
        // 데이터 찾기
        CharacterData CharacterSO = DataManager.instance.GetCharacterById(ID);
        Data = new CharacterRuntimeData(CharacterSO);
    }

    public void RequestDamaged(int Amount)
    {
        Data.TakeDamage(Amount);
        OnChangedState?.Invoke(Data);

        if(Data.IsDead)
        {
            Dead();
        }
    }

    private void Dead()
    {
        // 상태를 바꿀지 아님 죽음 처리할지 여기서 선택
    }

    #region DragInterfaceLogic
    void IActionDragHandler.OnHoverEnter()
    {
        if (Data.IsDead)
            return;

        if (BattleManager.instance.IsPlayerTurn())
        {
            _material.SetFloat("_Enable", 1);
        }
    }

    void IActionDragHandler.OnHoverExit()
    {
        if (BattleManager.instance.IsPlayerTurn())
        {
            _material.SetFloat("_Enable", 0);
        }
    }

    void IActionDragHandler.BeginDrag()
    {

    }

    void IActionDragHandler.OnDrop(MonoBehaviour DragItem)
    {
        if (Data.IsDead)
        {
            return;
        }

        var BattleMgr = BattleManager.instance;
        if (TurnManager.ETurnType.USE_CARDTRUN == BattleMgr.GetTurnType())
        {
            var CardUI = DragItem as CardUI;
            if (CardUI != null)
            {
                EventBus.Publish<UseCardEvent>(new UseCardEvent(this, CardUI));
            }
        }
        else if (TurnManager.ETurnType.ATTACK_ACTIONTURN == BattleMgr.GetTurnType())
        {
            BattleMgr.RequestAttack((Character)DragItem, this);
        }
    }

    void IActionDragHandler.EndDrag()
    {
    }

    void IActionDragHandler.OnHovering()
    {

    }
    #endregion
}
