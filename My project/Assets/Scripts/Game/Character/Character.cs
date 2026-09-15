using DG.Tweening;
using Spine;
using Spine.Unity;
using System;
using TurnCardGame.Data;
using UnityEngine;

public class Character : MonoBehaviour, IActionDragHandler
{
    #region Delegate
    public delegate void FinishedAction(EFSM_STATE state);
    public delegate void OnDagmaed(float fHealthRatio);
    public delegate void OnChangeState(CharacterRuntimeData Data);

    public event OnDagmaed OnDamaged;
    public event Action    OnDead;

    public event OnChangeState OnChangedState;
    public event FinishedAction OnFinishedAct;
    #endregion

    public bool bIsLeft = false;
    public CharacterRuntimeData     Data { get; protected set; }
    public float RotSpeed = 0.2f;

    protected FSM                   _CharacterFSM = null;
    protected AnimComponent         _AnimComponent = null;

    protected OutLineRenderer       _OutLineRender = null;
    protected SpriteRenderer        _spriteRender = null;
    protected bool                  _bIsAttackAble = false;

    protected Vector3               vOrizinLook = Vector3.right;
    protected Vector3               vOrizinPoint = Vector3.zero;
    
    private void Update()
    {
        _CharacterFSM?.UpdateFSM();
    }

    private void OnDestroy()
    {
        var animator = gameObject.GetComponent<SkeletonAnimation>();
        animator.AnimationState.Complete -= AnimFinished;
        _AnimComponent.RemoveListener(EFSM_STATE.ATTACK, AnimationCallbackEvent);
    }

    public void Initialize(CharacterData CharacterSO, Vector3 Position, bool IsEnemy)
    {
        transform.position = Position;
        bIsLeft = IsEnemy;

        var DataMgr = DataManager.instance;
        if (DataMgr == null)
            return;

        Data = new CharacterRuntimeData(CharacterSO);
        var AddressableMgr = AddressableManager.instance;
        var animator = gameObject.GetComponent<SkeletonAnimation>();

        _AnimComponent = gameObject.GetComponent<AnimComponent>();
        if(_AnimComponent != null )
            _AnimComponent.Initialize(CharacterSO, animator);

        _AnimComponent.AddListener(EFSM_STATE.ATTACK, AnimationCallbackEvent);

        animator.AnimationState.Complete += AnimFinished;
        if (_CharacterFSM == null)
            _CharacterFSM = GetComponent<FSM>();

        _CharacterFSM.Initialized(Data.Source.FSMConfig, this, _AnimComponent);
        
        _OutLineRender = gameObject.GetComponent<OutLineRenderer>();
        _OutLineRender.Initialize();
    }

    public void SetAttackAble(bool Active)
    {
        _bIsAttackAble = Active;
    }

    public void RequestDamaged(int Amount)
    {
        Data.TakeDamage(Amount);
        var PoolItem = PoolManager.Instance.Get<PoolAbleComponent>(GamePlay.Enum.EPoolType.Obejct, "DamageFont");
        if(PoolItem != null)
            PoolItem.gameObject.GetComponent<DamageFont>().Initalize(Amount, transform);

        if (Data.IsDead)
        {
            _CharacterFSM.ChangeState(EFSM_STATE.DEAD);
            OnDead?.Invoke();
        }
        else
            _CharacterFSM.ChangeState(EFSM_STATE.HIT);

        OnChangedState?.Invoke(Data);
    }

    public virtual void AttackAction(Vector3 vTargetPoint) { }

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

    }

    protected virtual void Attack() { }
    protected virtual void AnimFinished(TrackEntry entry) 
    {
        OnFinishedAct?.Invoke(_CharacterFSM._CurStateType);
    }

    void AnimationCallbackEvent(Spine.Event e)
    {
        if(e.Data.Name == "attack_hit")
            Attack();
    }

    protected void MoveTarget(Vector3 vTargetPoint, TweenCallback action)
    {
        _CharacterFSM.ChangeState(EFSM_STATE.RUN);

        vOrizinPoint = transform.position;
        transform.DOMove(vTargetPoint, 2.0f)
                 .OnComplete(action);
    }

    protected void ReverseLookAt()
    {
        float yAngle = transform.eulerAngles.y;

        yAngle = (yAngle + 180) % 360.0f;
        transform.DORotate(new Vector3(0, yAngle, 0), RotSpeed);
    }

    #region DragInterfaceLogic
    void IActionDragHandler.OnHoverEnter()
    {
        if (Data.IsDead)
            return;

        if (BattleManager.instance.IsPlayerTurn())
        {
            _OutLineRender.OnEnableOutLine(true);
        }
    }

    void IActionDragHandler.OnHoverExit()
    {
        if (BattleManager.instance.IsPlayerTurn())
        {
            _OutLineRender.OnEnableOutLine(false);
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
        if (ETurnType.USE_CARDTRUN == BattleMgr.GetTurnType())
        {
            var CardUI = DragItem as CardUI;
            if (CardUI != null)
            {
                EventBus.Publish<UseCardEvent>(new UseCardEvent(this, CardUI));
            }
        }
        else if (ETurnType.ATTACK_ACTIONTURN == BattleMgr.GetTurnType())
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
