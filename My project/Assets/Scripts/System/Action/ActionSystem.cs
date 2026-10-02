using GamePlay.Enum;
using UnityEngine;

public class ActionSystem
{
    public bool RequestUseCardAction(int ID, MonoBehaviour DragItem, CardUI UICard)
    {
        var BattleMgr = BattleManager.instance;
        var Enemy = BattleMgr.GetEnemyPartyList(ID);
        var UseCard = UICard._Data.CardData;

        var Target = DragItem as Character;
        if (Target == null)
            return false;

        if (!IsCanCardAction(ID, Target.OwnerID, UseCard.eTargetType))
            return false;

        switch (UseCard.eTargetType)
        {
            case ETargetType.FriendlySelf:
            case ETargetType.EnemySelf:
                EventBus.Publish<UseCardEvent>(new UseCardEvent(Target, UICard._Data.HandIndex, UICard._ControllerIdx, UseCard));
                break;

            case ETargetType.FriendlyAll:
            case ETargetType.EnemyAll:
                EventBus.Publish<UseCardEvent>(new UseCardEvent(UICard._Data.HandIndex, UICard._ControllerIdx, UseCard));
                break;
        }

        return true;
    }

    public bool RequestActtackdAction(Character Attacker, Character Enemy)
    {
        if (!IsCanAttackAction(Attacker, Enemy))
            return false;

        if (Enemy.Data.IsDead)
            return false;

        return true;
    }

    // Type에 맞게 확인해줌
    private bool IsCanCardAction(int ID, int TargetID, ETargetType CardType)
    {
        switch (CardType)
        {
            case ETargetType.FriendlySelf:
            case ETargetType.FriendlyAll:
                if (ID != TargetID)
                {
                    return false;
                }
                break;

            case ETargetType.EnemySelf:
            case ETargetType.EnemyAll:
                if (ID == TargetID)
                {
                    return false;
                }
                break;
        }

        return true;
    }

    private bool IsCanAttackAction(Character Attacker, Character Enemy)
    {
        if (Attacker.OwnerID == Enemy.OwnerID)
            return false;

        return true;
    }
}

