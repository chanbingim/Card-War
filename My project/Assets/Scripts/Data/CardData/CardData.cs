
using UnityEngine;

namespace TurnCardGame.Data
{
    public class UI_CardData
    {
        public int               HandIndex;
        public int               CardID;

        public readonly CardData CardData;

        public UI_CardData(int idx, int ID)
        {
            HandIndex = idx;
            CardID = ID;

            var DataMgr = DataManager.instance;
            if (DataMgr == null)
                CardData = null;
            else
                CardData = DataMgr.GetCardById(ID);
        }
    }
}
