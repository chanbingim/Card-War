using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace TurnCardGame.Data.AI
{
    public enum CardUseTargetPolicy
    {
        [XmlEnum("Self")]
        Self,

        [XmlEnum("LowestHpEnemy")]
        LowestHpEnemy
    }

    [Serializable]
    [XmlRoot("CardUsePatternData")]
    public struct CardUsePatternData
    {
        [XmlArray("Turns")]
        [XmlArrayItem("CardUseTurnData")]
        public List<CardUseTurnData> Turns;
    }

    [Serializable]
    public struct CardUseTurnData
    {
        [XmlAttribute("number")]
        public int Number;

        [XmlArray("CardUses")]
        [XmlArrayItem("CardUseActionData")]
        public List<CardUseActionData> CardUses;
    }

    [Serializable]
    public struct CardUseActionData
    {
        [XmlAttribute("cardId")]
        public int CardId;

        [XmlAttribute("targetPolicy")]
        public CardUseTargetPolicy TargetPolicy;
    }
}
