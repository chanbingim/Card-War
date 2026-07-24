using System.Collections.Generic;
using UnityEngine;

public class PlayerData : TurnParticipantBase
{
    public  List<int> Decks;
    public  List<int> Hands;
    public  List<int> Skills;
    public  List<Character> PlayerParty { get; protected set; }

    public  Queue<CardAction>   _ActQueues { get; private set; } = new Queue<CardAction>();
    private Queue<CardAction>   _OldActQueues = new Queue<CardAction>();
    private CardAction          _CurAction;

    private GameObject          _TransformParent;

    public PlayerData()
    {
        Decks = new List<int>(GAME_CONST.Const.MAX_DECK);
        Hands = new List<int>(GAME_CONST.Const.MAX_HAND);
        Skills = new List<int>(GAME_CONST.Const.MAX_SKILL);

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
            character.Initialize(ID);
            character.OnFinishedAct += Next_Action;
        }
    }

    public void Request_RemoveParty(Character character)
    {
        if (PlayerParty.Count <= 0)
            return;

        if (PlayerParty.Contains(character))
            PlayerParty.Remove(character);
    }

    public void ADD_ActQueue(CardAction Act)
    {
        _ActQueues.Enqueue(Act);
    }

    public void Next_Action()
    {
        if (_CurAction != null)
        {
            _OldActQueues.Enqueue(_CurAction);
            _CurAction = null;
        }

        if (_ActQueues.Count > 0)
        {
            _CurAction = _OldActQueues.Dequeue();
        }
    }

    public void Update_PlayerAction()
    {
        if (_CurAction != null)
        {
            _CurAction.ActObject.Update_Action(_CurAction);
        }
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

    void OnDisable()
    {
        foreach(var character  in PlayerParty)
            character.OnFinishedAct -= Next_Action;
    }
}
