using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using TurnCardGame.Data;

public class BattleCardManager
{
    Queue<int> drawQueue = new Queue<int>();
    private bool isProcessing = false;

    public async UniTask RequestDrawCard(int DrawCount)
    {
        drawQueue.Enqueue(DrawCount);
        if (isProcessing)
            return;

        DrawCard(drawQueue.Dequeue()).Forget();
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

    private async UniTaskVoid DrawCard(int DrawCount)
    {
        isProcessing = true;
        while (drawQueue.Count > 0)
        {
            int count = drawQueue.Dequeue();
            var ClientPlayer = BattleManager.instance.GetLoaclPlayer();
            for (int i = 0; i < DrawCount; i++)
            {
                var card = ClientPlayer.DrawCard();
                EventBus.Publish(new CardDrawEvent(card));

                await UniTask.Delay(150); // 드로우 템포
            }
        }
        isProcessing = false;
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
