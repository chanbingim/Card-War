using DG.Tweening;
using System;
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

    public CharacterRuntimeData Data { get; private set; }

    protected FSM             _CharacterFSM = null;
    
    protected SpriteRenderer  _spriteRender = null;
    protected Material        _material = null;
    protected bool            _bIsAttackAble = false;
    protected Vector3         vOrizinPoint = Vector3.zero;

    private void Awake()
    {
        _spriteRender = GetComponent<SpriteRenderer>();
        if(_spriteRender != null )
        {
            _material = _spriteRender.material;
        }
    }

    private void Update()
    {
        _CharacterFSM?.UpdateFSM();
    }

    public void Initialize(CharacterData CharacterSO, Vector3 Position)
    {
        transform.position = Position;

        var DataMgr = DataManager.instance;
        if (DataMgr == null)
            return;

        Data = new CharacterRuntimeData(CharacterSO);
        _spriteRender.sprite = DataMgr.GetCharacterSprite(CharacterSO.Id);

        var AddressableMgr = AddressableManager.instance;
        Animator animator = gameObject.AddComponent<Animator>();

        if(Data.Source.AnimControllerKey != null)
            animator.runtimeAnimatorController = AddressableMgr.Get<RuntimeAnimatorController>(Data.Source.AnimControllerKey);

        if (_CharacterFSM == null)
            _CharacterFSM = GetComponent<FSM>();

        _CharacterFSM.Initialized(Data.Source.FSMConfig, this, animator);
    }

    public void SetAttackAble(bool Active)
    {
        _bIsAttackAble = Active;
    }

    public void RequestDamaged(int Amount)
    {
        Data.TakeDamage(Amount);

        if(Data.IsDead)
            _CharacterFSM.ChangeState(EFSM_STATE.Dead);
        else
            _CharacterFSM.ChangeState(EFSM_STATE.Hit);

        OnChangedState?.Invoke(Data);
    }

    public virtual void AttackAction(Vector3 vTargetPoint) { }
   

    public virtual void AnimFinished()
    {
        if (_CharacterFSM._CurStateType == EFSM_STATE.Attack)
        {
            var Animator = _CharacterFSM._Animator;
            if (Animator != null)
            {
                _CharacterFSM.ChangeState(EFSM_STATE.Move);
                AnimatorStateInfo state = Animator.GetCurrentAnimatorStateInfo(0);
                transform.DOMove(vOrizinPoint, state.length)
                                         .OnComplete(() =>
                                         {
                                             _CharacterFSM.ChangeState(EFSM_STATE.Idle);
                                         });
            }
        }
        else if(_CharacterFSM._CurStateType == EFSM_STATE.Hit)
        {
            _CharacterFSM.ChangeState(EFSM_STATE.Idle);
        }
    }

    public virtual void Idle()
    {

    }

    public virtual void Move()
    {

    }

    public virtual void Hit()
    {
        // 파티클 재생
    }

    public virtual void Dead()
    {
        // 상태를 바꿀지 아님 죽음 처리할지 여기서 선택
        _CharacterFSM.ChangeState(EFSM_STATE.Dead);
    }

    protected virtual void Attack() { }

    protected void MoveTarget(Vector3 vTargetPoint, TweenCallback action)
    {
        vOrizinPoint = transform.position;
        _CharacterFSM.ChangeState(EFSM_STATE.Move);

        var Animator = _CharacterFSM._Animator;
        if (Animator != null)
        {
            AnimatorStateInfo state = Animator.GetCurrentAnimatorStateInfo(0);
            transform.DOMove(vTargetPoint, state.length)
                     .OnComplete(action);
        }
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
            /*if (_bIsAttackAble == false)
                return;*/

            BattleMgr.RequestAttack((Character)DragItem, this);
            _bIsAttackAble = false;
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
