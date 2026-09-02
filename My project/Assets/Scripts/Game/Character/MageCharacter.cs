using UnityEngine;

public class MageCharacter : Character
{
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

    public override void AttackAction(Vector3 vTargetPoint)
    {
        _CharacterFSM.ChangeState(EFSM_STATE.Attack);
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
