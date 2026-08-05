using System;
using System.Collections.Generic;
using TurnCardGame.Data;
using UnityEngine;

public class BattlePlayerData : TurnParticipantBase
{
    public  List<int>               Decks;
    public  List<int>               Hands;
    public  List<int>               Skills;
    public  List<Character>         PlayerParty { get; protected set; }

    private GameObject              TransformParent;
    private ActionQueue             ActionQueue = new ActionQueue();

    public BattlePlayerData()
    {
        Name = $"Sample Test Player {PlayerTurnIndex}";

        Decks = new List<int>(GAME_CONST.Const.MAX_DECK);
        Hands = new List<int>(GAME_CONST.Const.MAX_HAND);
        Skills = new List<int>(GAME_CONST.Const.MAX_SKILL);

        TransformParent = new GameObject(Name + "_Party");
        IsLocal = false;

        ADDSamplePlayerData();
    }
   
    public BattlePlayerData(PlayerData playerData, bool IsLocalPlayer = false)
    {
        Name = playerData.Name;
        Decks = new List<int>(playerData.Decks);
        Skills = new List<int>(playerData.Skills);
        Hands = new List<int>(GAME_CONST.Const.MAX_HAND);
        IsLocal = IsLocalPlayer;

        TransformParent = new GameObject(Name + "_Party");
    }

    public void Request_ADDParty(int ID, Vector3 WorldPosition = default)
    {
        GameObject Prefab = AddressableManager.instance.Get<GameObject>("Prefabs/Character"); // 여기서 어드레서블로 받는다.

        if (!Utility.CHECK(Prefab))
            return;

        var obj = GameObject.Instantiate(Prefab, TransformParent.transform);
        if (Utility.CHECK(obj) == false)
            return;

        Character character = obj.GetComponent<Character>();
        character.transform.position = WorldPosition;

        if (Utility.CHECK(character))
        {
            if (IsLocal == false)
                character.GetComponent<SpriteRenderer>().flipX = true;

            character.Initialize(ID);
            character.OnFinishedAct += ActionQueue.Next_Action;
        }
    }

    public UI_CardData DrawCard()
    {
        int ID = 0;
        int HandIndex = -1;
        if (Decks.Count > 0)
        {
            ID = Decks[0];
            Decks.RemoveAt(0);

            HandIndex = Hands.Count;
            Hands.Add(ID);
        }

        if (ID <= 0)
            return null;

        return new UI_CardData(HandIndex, ID);
    }

    public void UseCard(UseCardEvent card)
    {
        Hands.RemoveAt(card.UseCard._Data.HandIndex);

        var CardAct = new CardAction(PlayerTurnIndex, card.Target, EACTION_TYPE.ATTACK);
        ADD_ActQueue(CardAct);
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

    #region ActionQueue
    public void ADD_ActQueue(CardAction action)  
    {
        ActionQueue.ADD_ActQueue(action); 

    }
    public int  ActionCount() { return ActionQueue._ActQueues.Count; }
    public void Update_PlayerAction() { ActionQueue.Update_PlayerAction(); }
    #endregion

    private void OnDisable()
    {
        foreach (var character  in PlayerParty)
            character.OnFinishedAct -= ActionQueue.Next_Action;
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
            Request_ADDParty(1, stage.GetPlayerWorldPosition(Fomation.LocalPosition[0]));
            Request_ADDParty(2, stage.GetPlayerWorldPosition(Fomation.LocalPosition[1]));
        }
        else
        {
            Request_ADDParty(1);
            Request_ADDParty(2);
        }
    }

}
