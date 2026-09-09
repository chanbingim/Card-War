using DG.Tweening;
using Spine;
using Spine.Unity;
using System;
using TurnCardGame.Data;
using UnityEngine;
using static TurnManager;
using static UnityEngine.GraphicsBuffer;

public class Character : MonoBehaviour, IActionDragHandler
{
    #region Delegate
    public delegate void FinishedAction();
    public delegate void OnDagmaed(float fHealthRatio);
    public delegate void OnChangeState(CharacterRuntimeData Data);

    public event OnDagmaed OnDamaged;
    public event Action    OnDead;

    public event OnChangeState OnChangedState;
    public event FinishedAction OnFinishedAct;
    #endregion

    public CharacterRuntimeData Data { get; protected set; }
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

    private void OnDestroy()
    {
        var animator = gameObject.GetComponent<SkeletonAnimation>();
        animator.AnimationState.Complete -= AnimFinished;
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
        var animator = gameObject.GetComponent<SkeletonAnimation>();

        if(Data.Source.SkeletonDataKey != null)
            animator.skeletonDataAsset = AddressableMgr.Get<SkeletonDataAsset>(Data.Source.SkeletonDataKey);

        animator.AnimationState.Complete += AnimFinished;

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
        var PoolItem = PoolManager.Instance.Get<PoolAbleComponent>(GamePlay.Enum.EPoolType.Obejct, "DamageFont");
        if(PoolItem != null)
            PoolItem.gameObject.GetComponent<DamageFont>().Initalize(Amount, transform);

        if (Data.IsDead)
        {
            _CharacterFSM.ChangeState(EFSM_STATE.Dead);
            OnDead?.Invoke();
        }
        else
            _CharacterFSM.ChangeState(EFSM_STATE.Hit);

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
        // 상태를 바꿀지 아님 죽음 처리할지 여기서 선택
        _CharacterFSM.ChangeState(EFSM_STATE.Dead);
    }

    protected virtual void Attack() { }
    protected virtual void AnimFinished(TrackEntry entry) { }

    protected void MoveTarget(Vector3 vTargetPoint, TweenCallback action)
    {
        _CharacterFSM.ChangeState(EFSM_STATE.Move);

        vOrizinPoint = transform.position;
        Vector3 dir = vTargetPoint - vOrizinPoint;
        dir.Normalize();

        float cross = Vector3.Cross(transform.up, dir).z;
        if (dir.x > 0)
        {
            transform.DORotate(Vector3.zero, 0.2f);
        }
        else if (dir.x < 0)
        {
            transform.DORotate(new Vector3(0, 180, 0), 0.2f);
        }

        transform.DOMove(vTargetPoint, 2.0f)
                 .OnComplete(action);
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
