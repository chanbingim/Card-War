using System;
using TurnCardGame.Data;
using UnityEngine;
using UnityEngine.U2D;

public class PlayerDataManager : MonoBehaviour
{
    public PlayerData       LocalPlayer {get; private set;}
    public SpriteAtlas      SpriteAtlas;
    private Sprite[]        sprites;

    public event Action<int>            DrawCardEvent;
    public event Action<CardUI>         UseCardEvent;

    void Request_PlayerData()
    {
        LocalPlayer = new PlayerData();
        for (int i = 1; i <= GAME_CONST.Const.MAX_DECK; i++)
            LocalPlayer.Decks.Add(i);
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

    public void Use_Card(CardUI card)
    {
        UseCardEvent.Invoke(card);
        LocalPlayer.Hands.RemoveAt(card._Data.HandIndex);
    }

    public Sprite Get_CardImage(int ID) { return sprites[ID]; }


    #region Defualt
    static public PlayerDataManager instance { get; private set; }
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        instance.Initialize();
    }

    private void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }

    private void Initialize()
    {
        Request_PlayerData();

        sprites = new Sprite[SpriteAtlas.spriteCount];
        SpriteAtlas.GetSprites(sprites);
    }
    #endregion
}
