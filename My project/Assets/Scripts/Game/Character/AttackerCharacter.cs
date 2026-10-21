using Spine;
using Spine.Unity;
using TurnCardGame.Data;
using UnityEngine;

public class AttackerCharacter : Character
{
    [SerializeField] CharacterData SO = null;
    [SerializeField] SkeletonDataAsset skeletonDataAsset = null;
 

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            AttackAction(new Vector3(1, 1, 0));
        }

        _CharacterFSM?.UpdateFSM();
    }

    public override void AttackAction(Vector3 vTargetPoint)
    {
        MoveTarget(vTargetPoint, () =>
        {
            _CharacterFSM.ChangeState(EFSM_STATE.ATTACK);
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
        base.Hit();


    }

    public override void Dead()
    {
        // 상태를 바꿀지 아님 죽음 처리할지 여기서 선택
        _CharacterFSM.ChangeState(EFSM_STATE.DEAD);
    }

    protected override void AnimFinished(TrackEntry entry)
    {
        if (_CharacterFSM._CurStateType == EFSM_STATE.ATTACK)
        {
            ReverseLookAt();
            MoveTarget(vOrizinPoint, () =>
            {
                ReverseLookAt();
                _CharacterFSM.ChangeState(EFSM_STATE.IDLE);
                base.AnimFinished(entry);
            });
        }
        else if (_CharacterFSM._CurStateType == EFSM_STATE.HIT)
        {
            _CharacterFSM.ChangeState(EFSM_STATE.IDLE);
        }
    }

    protected override void Attack()
    {
        if (_CharacterFSM._CurStateType == EFSM_STATE.ATTACK)
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

            int Damage = Data.CurrentATKPower + (_buffComponent?.GetTotalEffectValue(ECardEffectType.AttackBuff) ?? 0);
            Damage = BattleMgr.ComputeDamageLogic(Damage);

            var SkillInfo = GetSkill();
            CurBattle.TargetObject.RequestDamaged(SkillInfo);

            DataManager.instance.TryCameraData(SkillInfo.AttackerCameraShakeId, out var CamShakeData);
            BattleMgr.ShakeCamera("MainCam", CamShakeData);

            /// 히트스탑을 처리하는데 여기서 한번 출력을 해보자
            UIManager.instance.SlpashPlay();

            HitStop(0.3f);
            CurBattle.TargetObject.HitStop(0.3f);
        }
    }
}
