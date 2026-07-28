using System;
using TurnCardGame.Data;
using UnityEngine;

public class BattleCardManager
{
    public  BattlePlayerData        LocalPlayer {get; private set;}

    public bool IsPlayerTurn() { return LocalPlayer.IsActive; }
    public void PlayerTrunEnd()
    {
        LocalPlayer.TurnEnd();
    }

    public UI_CardData Draw_Card()
    {
        int ID = 0;
        int HandIndex = -1;
        if (LocalPlayer.Decks.Count > 0)
        {
            ID = LocalPlayer.Decks[0];
            LocalPlayer.Decks.RemoveAt(0);

            HandIndex = LocalPlayer.Hands.Count;
            LocalPlayer.Hands.Add(ID);
        }

        if (ID <= 0)
            return null;

        return new UI_CardData(HandIndex, ID);
    }

    public void Use_Card(UseCardEvent card)
    {
        LocalPlayer.Hands.RemoveAt(card.UseCard._Data.HandIndex);

        var CardAct = new CardAction(card.Target, EACTION_TYPE.ATTACK);
        LocalPlayer.ADD_ActQueue(CardAct);
    }

    #region Default
    /* 객체를 오래 소유하는 것은 피할것 */
    public static BattleCardManager Create(Stage stage)
    {
        BattleCardManager instance = new BattleCardManager();
        if(instance.Initialize(stage) == false)
            return null;

        return instance;
    }

    private bool Initialize(Stage stage)
    {
        Request_PlayerData(stage, GameClientManager.instance.playerData);
        EventBus.Subscribe<UseCardEvent>(Use_Card);

        return true;
    }

    void Request_PlayerData(Stage stage, PlayerData playerData)
    {
        AddressableManager AddressableMgr = AddressableManager.instance;
        if(AddressableMgr == null)
            throw new ArgumentException("어드레서블 매니저 생성 필요");

        var Fomation = AddressableMgr.Get<FormationSO>("Formation/ThreeFormation");
        LocalPlayer = new BattlePlayerData(playerData);
        LocalPlayer.SetName("Player 0");

        for (int i = 1; i <= GAME_CONST.Const.MAX_DECK; i++)
            LocalPlayer.Decks.Add(i % 4);

        if (Fomation != null)
        {
            LocalPlayer.Request_ADDParty(1, stage.GetPlayerWorldPosition(Fomation.LocalPosition[0]));
            LocalPlayer.Request_ADDParty(2, stage.GetPlayerWorldPosition(Fomation.LocalPosition[1]));
        }
        else
        {
            LocalPlayer.Request_ADDParty(1);
            LocalPlayer.Request_ADDParty(2);
        }
    }
    #endregion
}
