using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattlePlayerData : TurnParticipantBase
{
    public  List<int>               Decks;
    public  List<int>               Hands;
    public  List<int>               Skills;
    public  List<Character>         PlayerParty { get; protected set; }

    private GameObject                  _TransformParent;
    private ActionQueue                 _ActionQueue = new ActionQueue();

    public BattlePlayerData()
    {
        Decks = new List<int>(GAME_CONST.Const.MAX_DECK);
        Hands = new List<int>(GAME_CONST.Const.MAX_HAND);
        Skills = new List<int>(GAME_CONST.Const.MAX_SKILL);

        _TransformParent = new GameObject(Name + "_Party");
        IsLocal = false;
    }

    public BattlePlayerData(PlayerData playerData, bool IsLocalPlayer = false)
    {
        Decks = new List<int>(playerData.Decks);
        Skills = new List<int>(playerData.Skills);
        Hands = new List<int>(GAME_CONST.Const.MAX_HAND);
        IsLocal = IsLocalPlayer;

        _TransformParent = new GameObject(Name + "_Party");
    }

    public void Request_ADDParty(int ID, Vector3 WorldPosition = default)
    {
        GameObject Prefab = AddressableManager.instance.Get<GameObject>("Prefabs/Character"); // 여기서 어드레서블로 받는다.

        if (!Utility.CHECK(Prefab))
            return;

        var obj = GameObject.Instantiate(Prefab, _TransformParent.transform);
        if (Utility.CHECK(obj) == false)
            return;

        Character character = obj.GetComponent<Character>();
        character.transform.position = WorldPosition;

        if (Utility.CHECK(character))
        {
            if (IsLocal == false)
                character.GetComponent<SpriteRenderer>().flipX = true;

            character.Initialize(ID);
            character.OnFinishedAct += _ActionQueue.Next_Action;
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

    public void SetName(string name)
    {
        Name = name;
    }

    #region ActionQueue
    public void ADD_ActQueue(CardAction action)  { _ActionQueue.ADD_ActQueue(action); }
    public int  ActionCount() { return _ActionQueue._ActQueues.Count; }
    public void Update_PlayerAction() { _ActionQueue.Update_PlayerAction(); }
    #endregion

    void OnDisable()
    {
        foreach (var character  in PlayerParty)
            character.OnFinishedAct -= _ActionQueue.Next_Action;
    }
}
