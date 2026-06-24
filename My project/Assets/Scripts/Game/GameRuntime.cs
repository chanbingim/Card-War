using System.Collections.Generic;
using System.Reflection;
using TurnCardGame.Data;
using UnityEngine;

namespace TurnCardGame.Game
{
    public sealed class GameRuntime : MonoBehaviour
    {
        private static GameRuntime instance;

        [SerializeField] private List<StageData> configuredStages = new List<StageData>();

        public static GameRuntime Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindFirstObjectByType<GameRuntime>();
                }

                if (instance == null)
                {
                    instance = new GameObject("Game Runtime", typeof(GameRuntime)).GetComponent<GameRuntime>();
                }

                return instance;
            }
        }

        public GameSession Session { get; private set; }

        public IReadOnlyList<StageData> Stages
        {
            get
            {
                EnsureSession();
                return Session.Stages;
            }
        }

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);
            EnsureSession();
        }

        public void ConfigureStages(IEnumerable<StageData> stages)
        {
            configuredStages = new List<StageData>();
            if (stages != null)
            {
                foreach (StageData stage in stages)
                {
                    if (stage != null)
                    {
                        configuredStages.Add(stage);
                    }
                }
            }

            Session = new GameSession(configuredStages.Count > 0 ? configuredStages : CreateFallbackStages());
        }

        public void BeginNewGame()
        {
            EnsureSession();
            Session.StartGame();
        }

        public void SelectStage(StageData stage)
        {
            EnsureSession();
            if (Session.Phase == GamePhase.Start)
            {
                Session.StartGame();
            }

            Session.SelectStage(stage);
        }

        public void ReturnToStageSelect()
        {
            EnsureSession();
            Session.StartGame();
        }

        private void EnsureSession()
        {
            if (Session != null)
            {
                return;
            }

            Session = new GameSession(configuredStages.Count > 0 ? configuredStages : CreateFallbackStages());
        }

        private static IReadOnlyList<StageData> CreateFallbackStages()
        {
            CardData strike = CreateCard("strike", "Strike", CardEffectType.Damage, 6, "Deal 6 damage.");
            CardData guard = CreateCard("guard", "Guard", CardEffectType.Guard, 3, "Gain 3 guard.");
            CardData mend = CreateCard("mend", "Mend", CardEffectType.Heal, 4, "Restore 4 health.");

            MonsterData slime = CreateMonster("slime", "Training Slime", 14, 2);
            MonsterData knight = CreateMonster("knight", "Rust Knight", 20, 4);

            StageData first = ScriptableObject.CreateInstance<StageData>();
            SetPrivateField(first, "stageId", "stage_001");
            SetPrivateField(first, "title", "Stage 1 - Training Field");
            SetPrivateField(first, "monsters", new List<MonsterData> { slime });
            SetPrivateField(first, "startingDeck", new List<CardData> { strike, strike, guard, mend });

            StageData second = ScriptableObject.CreateInstance<StageData>();
            SetPrivateField(second, "stageId", "stage_002");
            SetPrivateField(second, "title", "Stage 2 - Old Gate");
            SetPrivateField(second, "monsters", new List<MonsterData> { knight });
            SetPrivateField(second, "startingDeck", new List<CardData> { strike, strike, strike, guard, mend });

            return new[] { first, second };
        }

        private static CardData CreateCard(string id, string title, CardEffectType type, int power, string description)
        {
            CardData card = ScriptableObject.CreateInstance<CardData>();
            SetPrivateField(card, "cardId", id);
            SetPrivateField(card, "title", title);
            SetPrivateField(card, "effectType", type);
            SetPrivateField(card, "power", power);
            SetPrivateField(card, "description", description);
            return card;
        }

        private static MonsterData CreateMonster(string id, string title, int maxHealth, int attack)
        {
            MonsterData monster = ScriptableObject.CreateInstance<MonsterData>();
            SetPrivateField(monster, "monsterId", id);
            SetPrivateField(monster, "title", title);
            SetPrivateField(monster, "maxHealth", maxHealth);
            SetPrivateField(monster, "attackPower", attack);
            return monster;
        }

        private static void SetPrivateField<TTarget, TValue>(TTarget target, string fieldName, TValue value)
        {
            FieldInfo field = typeof(TTarget).GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            field?.SetValue(target, value);
        }
    }
}
