using System;
using TurnCardGame.Data;

public class BattleCardManager
{
    public UI_CardData Draw_Card()
    {
        var ClientPlayer = BattleManager.instance.GetLoaclPlayer();
        return ClientPlayer.DrawCard();
    }

    public void Use_Card(UseCardEvent card)
    {
        var ClientPlayer = BattleManager.instance.GetLoaclPlayer();
        ClientPlayer.UseCard(card);
    }

    public void Relese()
    {
        EventBus.Unsubscribe<UseCardEvent>(Use_Card);
    }

    #region Default
    /* 객체를 오래 소유하는 것은 피할것 */
    public static BattleCardManager Create()
    {
        BattleCardManager instance = new BattleCardManager();
        if(instance.Initialize() == false)
            return null;

        return instance;
    }

    private bool Initialize()
    {
        EventBus.Subscribe<UseCardEvent>(Use_Card);

        return true;
    }
    #endregion
}
