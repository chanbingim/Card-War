
using UnityEngine;

namespace TurnCardGame.Data
{
    public class UI_CardData
    {
        public int      HandIndex;
        public int      CardID;
        public Sprite   sprite;

        public UI_CardData(int idx, int ID)
        {
           HandIndex = idx;
           CardID = ID;
        }
    }
}
