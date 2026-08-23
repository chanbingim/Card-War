using DG.Tweening;
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

    private FSM             _CharacterFSM = null;

    private Material        _material = null;
    private bool            _bIsAttackAble = false;
    private Vector3         vOrizinPoint = Vector3.zero;

    private void Start()
    {
        SpriteRenderer spriteRender = GetComponent<SpriteRenderer>();
        if(spriteRender != null )
        {
            _material = spriteRender.material;
        }
    }

    private void Update()
    {
        _CharacterFSM?.UpdateFSM();
    }

    public void Initialize(int CharacterID, Vector3 Position)
    {
        transform.position = Position;
        ID = CharacterID;

        // 데이터 찾기
        CharacterData CharacterSO = DataManager.instance.GetCharacterById(ID);
        Data = new CharacterRuntimeData(CharacterSO);

        var AddressableMgr = AddressableManager.instance;
        Animator animator = gameObject.AddComponent<Animator>();

        if(Data.Source.AnimControllerKey != null)
            animator.runtimeAnimatorController = AddressableMgr.Get<RuntimeAnimatorController>(Data.Source.AnimControllerKey);

        if (_CharacterFSM == null)
            _CharacterFSM = GetComponent<FSM>();

        _CharacterFSM.Initialized(Data.Source.FSMConfig, this, animator);
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

    public void MoveTarget(Vector3 vTargetPoint)
    {
        vOrizinPoint = transform.position;
        _CharacterFSM.ChangeState(EFSM_STATE.Move);

        var Animator = _CharacterFSM._Animator;
        if (Animator != null)
        {
            AnimatorStateInfo state = Animator.GetCurrentAnimatorStateInfo(0);
            transform.DOMove(vTargetPoint, state.length)
                     .OnComplete(() =>
                     {
                         _CharacterFSM.ChangeState(EFSM_STATE.Attack);
                     });
        }
    }

    public void Attack()
    {
        if(_CharacterFSM._CurStateType == EFSM_STATE.Attack)
        {
            var BattleMgr = BattleManager.instance;
            if(BattleMgr == null)
            {
                Debug.LogWarning("[Character] not Find Battle Manager");
                return;
            }

            var CurBattle = BattleMgr._CurBattleAction;
            if (CurBattle == null)
                return;

            int Damage = BattleMgr.ComputeDamageLogic(Data.CurrentATKPower);
            CurBattle.TargetObject.RequestDamaged(Damage);
            AnimFinished();
        }
    }

    public void AnimFinished()
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
    }

    public virtual void Idle()
    {

    }

    public virtual void Move()
    {

    }

    public virtual void Hit()
    {

    }

    public virtual void Dead()
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
