using DG.Tweening;
using Spine;
using Spine.Unity;
using TurnCardGame.Data;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class AttackerCharacter : Character
{
    [SerializeField] bool DebugMode = false;
    [SerializeField] CharacterData SO = null;
    [SerializeField] SkeletonDataAsset skeletonDataAsset = null;

    private void Awake()
    {
        _spriteRender = GetComponent<SpriteRenderer>();
        if(_spriteRender != null )
        {
            _material = _spriteRender.material;
        }

        if (DebugMode)
        {
            var animator = gameObject.GetComponent<SkeletonAnimation>();
            animator.skeletonDataAsset = skeletonDataAsset;

            animator.AnimationState.Complete += AnimFinished;
            Data = new CharacterRuntimeData(SO);
            
            _CharacterFSM = GetComponent<FSM>();

            _CharacterFSM.Initialized(Data.Source.FSMConfig, this, animator);
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            AttackAction(Vector3.one);
        }

        _CharacterFSM?.UpdateFSM();
    }

    public override void AttackAction(Vector3 vTargetPoint)
    {
        MoveTarget(vTargetPoint, () =>
        {
            _CharacterFSM.ChangeState(EFSM_STATE.Attack);
        });
    }

    public override void Idle()
    {

    }

    public override void Move()
    {

    }

    public override void Hit()
    {
        // 파티클 재생
    }

    public override void Dead()
    {
        // 상태를 바꿀지 아님 죽음 처리할지 여기서 선택
        _CharacterFSM.ChangeState(EFSM_STATE.Dead);
    }

    protected override void AnimFinished(TrackEntry entry)
    {
        if (_CharacterFSM._CurStateType == EFSM_STATE.Attack)
        {
            MoveTarget(vOrizinPoint, () =>
            {
                transform.DORotate(Vector3.zero, 0.2f);
                _CharacterFSM.ChangeState(EFSM_STATE.Idle);
            });
        }
        else if (_CharacterFSM._CurStateType == EFSM_STATE.Hit)
        {
            _CharacterFSM.ChangeState(EFSM_STATE.Idle);
        }
    }

    protected override void Attack()
    {
        if (_CharacterFSM._CurStateType == EFSM_STATE.Attack)
        {
            var BattleMgr = BattleManager.instance;
            if (BattleMgr == null)
            {
                Debug.LogWarning("[Character] not Find Battle Manager");
                return;
            }

            var CurBattle = BattleMgr._CurBattleAction;
            if (CurBattle == null)
                return;

            int Damage = BattleMgr.ComputeDamageLogic(Data.CurrentATKPower);
            CurBattle.TargetObject.RequestDamaged(Damage);
        }
    }
}
