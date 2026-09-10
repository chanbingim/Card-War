using System;
using Unity.Behavior;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "AttackActionNode", story: "AttackAction", category: "Action", id: "e8f88ec07e2de88376c57b5209b99574")]
public partial class AttackActionNode : Action
{
    [SerializeReference]
    public BlackboardVariable<AIController> _Controller;

    private BattleManager       _BattleMgr;
    private BattlePlayerData    _PlayerData;

    HashSet<(Character, Character)> _CombatList = new ();

    bool IsAttackAble = true;
    float CurTime = 0;

    protected override void OnSetup()
    {
        CurTime = 0;
        _PlayerData = _Controller.Value._Data;

        _BattleMgr = BattleManager.instance;
        if (_BattleMgr == null)
        {
            Debug.Log("[AttackActionNode] Not Find BattlgeManager");
        }

        foreach (var item in _PlayerData.PlayerParty)
        {
            item.OnFinishedAct += FinishedAction;
        }
    }

    protected override Status OnStart()
    {
        MakeTargetList();
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        CurTime += Time.deltaTime;

        if(CurTime > 10.0f)
        {
            Debug.Log("[AttackActionNode] Turn Time Out");
            CurTime = 0;
            return Status.Success;
        }
        else if(CurTime >= 1.0f)
        {
            Debug.Log("[AttackActionNode] Attack Action");
            if(IsAttackAble && _CombatList.Count > 0)
            {
                var hash = _CombatList.First();
                _BattleMgr.RequestAttack(hash.Item1, hash.Item2);

                IsAttackAble = false;
            }
        }

        return Status.Running;
    }

    protected override void OnEnd()
    {
        _CombatList.Clear();
    }

    public void OnDestroy()
    {
        foreach (var item in _PlayerData.PlayerParty)
        {
            item.OnFinishedAct -= FinishedAction;
        }
    }

    private void FinishedAction()
    {
        if (IsAttackAble == false)
            IsAttackAble = true;
    }

    private void MakeTargetList()
    {
        List<Character> MyParty = new List<Character>(_PlayerData.PlayerParty);
        List<Character> EnmeyList = new List<Character>(_BattleMgr.GetEnemyPartyList(_PlayerData.PlayerTurnIndex));

        while (EnmeyList.Count > 0 || MyParty.Count > 0)
        {
            var Character = _BattleMgr.RequestRandomTarget(MyParty);
            var EnemyCharater = _BattleMgr.RequestRandomTarget(EnmeyList);

            MyParty.Remove(Character);
            EnmeyList.Remove(EnemyCharater);

            _CombatList.Add((Character, EnemyCharater));
        }
    }
}

