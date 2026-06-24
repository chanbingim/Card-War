using System.Collections.Generic;
using TurnCardGame.Data;
using TurnCardGame.Game;
using TurnCardGame.UI.Components;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TurnCardGame.UI.Screens
{
    public sealed class GameAppBootstrap : MonoBehaviour
    {
        [SerializeField] private bool startInSampleTest;

        private GameSession session;
        private RectTransform root;
        private Text statusText;
        private Text logText;

        private void Awake()
        {
            EnsureEventSystem();
            session = new GameSession(CreateSampleStages());
            BuildCanvas();
            if (startInSampleTest)
            {
                session.StartGame();
                session.SelectStage(session.Stages[0]);
                ShowBattle(new List<string> { "Scene sample test loaded. Use card buttons, Sample Turn, or Resolve Combat." });
            }
            else
            {
                ShowStart();
            }
        }

        private void ShowStart()
        {
            ClearRoot();
            AddTitle("Turn Card Game");
            AddBody("Draw two cards each turn, record actions, then resolve player and monster combat.");
            PrimaryButton.Create(root, "Start Game", () =>
            {
                session.StartGame();
                ShowStageSelect();
            });
            PrimaryButton.Create(root, "Play Sample Test", () =>
            {
                session.StartGame();
                session.SelectStage(session.Stages[0]);
                ShowBattle(new List<string> { "Sample test started. Try Sample Turn to resolve quickly." });
            });
        }

        private void ShowStageSelect()
        {
            ClearRoot();
            AddTitle("Stage Select");
            AddBody("Choose a stage. Clearing it returns here for the next run.");

            foreach (StageData stage in session.Stages)
            {
                PrimaryButton.Create(root, stage.Title, () =>
                {
                    session.SelectStage(stage);
                    ShowBattle(new List<string> { "Player turn started. Drew 2 cards." });
                });
            }
        }

        private void ShowBattle(IReadOnlyList<string> logLines = null)
        {
            ClearRoot();
            AddTitle(session.CurrentStage.Title);
            statusText = AddBody(BuildBattleStatus());
            AddSection("Monsters");
            foreach (MonsterState monster in session.Monsters)
            {
                AddBody($"{monster.Data.Title}: {monster.Health}/{monster.Data.MaxHealth} HP");
            }

            AddSection("Hand");
            for (int i = 0; i < session.Hand.Count; i++)
            {
                int handIndex = i;
                CardData card = session.Hand[i];
                PrimaryButton.Create(root, $"{card.Title} ({card.Power})", () =>
                {
                    session.UseCard(handIndex, session.FirstLiveMonster());
                    ShowBattle(new List<string> { $"{card.Title} recorded." });
                });
            }

            AddSection("Recorded Actions");
            if (session.PendingActions.Count == 0)
            {
                AddBody("No cards recorded yet.");
            }
            else
            {
                foreach (CardActionRecord action in session.PendingActions)
                {
                    AddBody($"{action.Card.Title} -> {action.TargetId}");
                }
            }

            PrimaryButton.Create(root, "Sample Turn", () =>
            {
                IReadOnlyList<string> result = PlaySampleTurn();
                if (session.Phase == GamePhase.StageCleared)
                {
                    ShowStageCleared(result);
                }
                else
                {
                    ShowBattle(result);
                }
            });
            PrimaryButton.Create(root, "Resolve Combat", () =>
            {
                IReadOnlyList<string> result = session.ResolveCombat();
                if (session.Phase == GamePhase.StageCleared)
                {
                    ShowStageCleared(result);
                }
                else
                {
                    ShowBattle(result);
                }
            });

            logText = AddBody(logLines == null ? string.Empty : string.Join("\n", logLines));
        }

        private IReadOnlyList<string> PlaySampleTurn()
        {
            int cardsToUse = session.Hand.Count;
            for (int i = 0; i < cardsToUse; i++)
            {
                session.UseCard(0, session.FirstLiveMonster());
            }

            var logLines = new List<string> { $"Sample turn recorded {cardsToUse} cards." };
            logLines.AddRange(session.ResolveCombat());
            return logLines;
        }

        private void ShowStageCleared(IReadOnlyList<string> logLines)
        {
            ClearRoot();
            AddTitle("Stage Cleared");
            AddBody(string.Join("\n", logLines));
            PrimaryButton.Create(root, "Back To Lobby", ShowStageSelect);
        }

        private string BuildBattleStatus()
        {
            return $"Phase: {session.Phase}\nPlayer HP: {session.PlayerHealth}\nHand: {session.Hand.Count}/5";
        }

        private void BuildCanvas()
        {
            var canvasObject = new GameObject("Runtime Game UI", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            var canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            var scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;

            var rootObject = new GameObject("Root", typeof(RectTransform), typeof(VerticalLayoutGroup), typeof(ContentSizeFitter));
            rootObject.transform.SetParent(canvasObject.transform, false);
            root = rootObject.GetComponent<RectTransform>();
            root.anchorMin = new Vector2(0.5f, 0.5f);
            root.anchorMax = new Vector2(0.5f, 0.5f);
            root.pivot = new Vector2(0.5f, 0.5f);
            root.sizeDelta = new Vector2(900f, 900f);

            var layout = rootObject.GetComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(32, 32, 28, 28);
            layout.spacing = 12f;
            layout.childAlignment = TextAnchor.UpperCenter;
            layout.childControlWidth = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;

            rootObject.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        }

        private void ClearRoot()
        {
            for (int i = root.childCount - 1; i >= 0; i--)
            {
                Destroy(root.GetChild(i).gameObject);
            }
        }

        private Text AddTitle(string text)
        {
            Text label = AddText(text, 42, FontStyle.Bold, TextAnchor.MiddleCenter);
            label.color = new Color(0.06f, 0.08f, 0.1f);
            return label;
        }

        private void AddSection(string text)
        {
            Text label = AddText(text, 24, FontStyle.Bold, TextAnchor.MiddleCenter);
            label.color = new Color(0.12f, 0.2f, 0.24f);
        }

        private Text AddBody(string text)
        {
            Text label = AddText(text, 18, FontStyle.Normal, TextAnchor.MiddleCenter);
            label.color = new Color(0.16f, 0.18f, 0.2f);
            return label;
        }

        private Text AddText(string text, int fontSize, FontStyle fontStyle, TextAnchor alignment)
        {
            var labelObject = new GameObject("Text", typeof(RectTransform), typeof(Text));
            labelObject.transform.SetParent(root, false);
            Text label = labelObject.GetComponent<Text>();
            label.text = text;
            label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            label.fontSize = fontSize;
            label.fontStyle = fontStyle;
            label.alignment = alignment;
            label.horizontalOverflow = HorizontalWrapMode.Wrap;
            label.verticalOverflow = VerticalWrapMode.Overflow;
            labelObject.GetComponent<RectTransform>().sizeDelta = new Vector2(820f, 0f);
            labelObject.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            return label;
        }

        private static void EnsureEventSystem()
        {
            if (FindFirstObjectByType<EventSystem>() == null)
            {
                new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
            }
        }

        private static IReadOnlyList<StageData> CreateSampleStages()
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
            typeof(TTarget).GetField(fieldName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)?.SetValue(target, value);
        }
    }
}
