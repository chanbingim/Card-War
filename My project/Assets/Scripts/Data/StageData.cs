using System.Collections.Generic;
using UnityEngine;

namespace TurnCardGame.Data
{
    [CreateAssetMenu(menuName = "Turn Card Game/Stage", fileName = "StageData")]
    public sealed class StageData : ScriptableObject
    {
        [SerializeField] private string stageId = "stage";
        [SerializeField] private string title = "Stage";
        [SerializeField] private List<MonsterData> monsters = new List<MonsterData>();
        [SerializeField] private List<CardData> startingDeck = new List<CardData>();

        public string StageId => stageId;
        public string Title => title;
        public IReadOnlyList<MonsterData> Monsters => monsters;
        public IReadOnlyList<CardData> StartingDeck => startingDeck;
    }
}
