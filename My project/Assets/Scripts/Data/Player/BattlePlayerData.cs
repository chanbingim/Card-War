using System;
using System.Collections.Generic;
using System.Linq;
using TurnCardGame.Data;
using UnityEngine;

public class BattlePlayerData : TurnParticipantBase
{
    public  List<int>               Decks;
    public  List<UI_CardData>       Hands;
    public  List<int>               Skills;
    public  List<Character>         PlayerParty { get; protected set; } = new List<Character>();

    private GameObject              TransformParent;

    public BattlePlayerData()
    {
        Name = $"Sample Test Player {PlayerTurnIndex}";

        Decks = new List<int>(GAME_CONST.Const.MAX_DECK);
        Hands = new List<UI_CardData>(GAME_CONST.Const.MAX_HAND);
        Skills = new List<int>(GAME_CONST.Const.MAX_SKILL);

        TransformParent = new GameObject(Name + "_Party");
        IsLocal = false;

        ADDSamplePlayerData();
        EventBus.Subscribe<ChangeTurnActEvent>(ChangeAttackAble);
    }
   
    public BattlePlayerData(PlayerData playerData, bool IsLocalPlayer = false)
    {
        Name = playerData.Name;
        Decks = new List<int>(playerData.Decks);
        Skills = new List<int>(playerData.Skills);
        Hands = new List<UI_CardData>(GAME_CONST.Const.MAX_HAND);
        IsLocal = IsLocalPlayer;

        TransformParent = new GameObject(Name + "_Party");
        ADDSamplePlayerData();
        EventBus.Subscribe<ChangeTurnActEvent>(ChangeAttackAble);
    }

    public void Request_ADDParty(int ID, Vector3 WorldPosition = default)
    {
        var Character = Factory.CharacterCreateFactory.Create(
         ID,
         TransformParent.transform,
         WorldPosition,
         IsLocal);

        if (Character == null)
            return;

        PlayerParty.Add(Character);
    }

    public UI_CardData DrawCard()
    {
        int ID = 0;
        int HandIndex = -1;

        if (Decks.Count > 0)
        {
            ID = Decks[0];
            Decks.RemoveAt(0);

            if (Hands.Count < GAME_CONST.Const.MAX_HAND)
            {
                HandIndex = Hands.Count;
                Hands.Add(new UI_CardData(HandIndex, ID));
            }
            else
                return new UI_CardData(HandIndex, ID);
        }

        if (ID <= 0)
            return null;

        return Hands.Last();
    }

    public void UseCard(UseCardEvent card)
    {
        TurnManager.ETurnType TrunType = BattleManager.instance.GetTurnType();
        if (TrunType == TurnManager.ETurnType.USE_CARDTRUN)
        {
            Hands.Remove(card.UseCard._Data);
            var CardAct = new CardAction(PlayerTurnIndex, card.Target, card.UseCard._Data, EACTION_TYPE.USE_CARD);
            EventBus.Publish<CardActionEvent>(new CardActionEvent(CardAct));
        }
    }

    public void Request_RemoveParty(Character character)
    {
        if (PlayerParty.Count <= 0)
            return;

        if (PlayerParty.Contains(character))
            PlayerParty.Remove(character);
    }

    public override void TurnRunning()
    {
        if (IsActive == false)
            return;



    }

    private void OnDisable()
    {

    }

    private void ChangeAttackAble(ChangeTurnActEvent turnStartEvent)
    {
        bool Active = turnStartEvent.eTurnType == TurnManager.ETurnType.ATTACK_ACTIONTURN ? 
                        true : false;

        foreach (var Character in PlayerParty)
        {
            Character.SetAttackAble(Active);
        }
    }

    private void ADDSamplePlayerData()
    {
        for (int i = 1; i <= GAME_CONST.Const.MAX_DECK; i++)
            Decks.Add(i % 4);

        var stage = BattleManager.instance.GetCurrentStage();
        var AddressableMgr = AddressableManager.instance;
        if (AddressableMgr == null)
            return;

        var Fomation = AddressableMgr.Get<FormationSO>("Formation/ThreeFormation");
        if (Fomation != null)
        {
            if(IsLocal)
            {
                Request_ADDParty(1, stage.GetPlayerWorldPosition(Fomation.LocalPosition[0]));
                Request_ADDParty(2, stage.GetPlayerWorldPosition(Fomation.LocalPosition[1]));
            }
            else
            {
                Request_ADDParty(3, stage.GetEnemyWorldPosition(Fomation.LocalPosition[0]));
                Request_ADDParty(4, stage.GetEnemyWorldPosition(Fomation.LocalPosition[1]));
            }
            
        }
        else
        {
            Request_ADDParty(1);
            Request_ADDParty(2);
        }
    }

}
