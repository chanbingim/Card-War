using System.Collections.Generic;
using TurnCardGame.Data;
using TurnCardGame.Game;
using TurnCardGame.UI.Components;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TurnCardGame.UI.Screens
{
    public sealed class BattleScreenController : MonoBehaviour
    {
        private RectTransform root;
        private List<string> logLines = new List<string>();

        private void Awake()
        {
            if (GameRuntime.Instance.Session.CurrentStage == null)
            {
                SceneManager.LoadScene(SceneNames.StageSelect);
                return;
            }

            Render(new[] { "Player turn started. Use cards, then resolve combat." });
        }

        private void Render(IReadOnlyList<string> latestLog = null)
        {
            if (latestLog != null)
            {
                logLines = new List<string>(latestLog);
            }

            if (root != null)
            {
                Destroy(root.parent.gameObject);
            }

            GameSession session = GameRuntime.Instance.Session;
            root = ScreenLayoutBuilder.CreateScreen("Battle");
            ScreenLayoutBuilder.AddTitle(root, session.CurrentStage.Title);
            ScreenLayoutBuilder.AddBody(root, $"Phase: {session.Phase}    Player HP: {session.PlayerHealth}    Hand: {session.Hand.Count}/5");

            RectTransform columns = ScreenLayoutBuilder.AddRow(root, "Battle Columns", 430f);
            RectTransform monsterColumn = ScreenLayoutBuilder.AddColumn(columns, "Monster Column", 330f);
            RectTransform handColumn = ScreenLayoutBuilder.AddColumn(columns, "Hand Column", 430f);
            RectTransform actionColumn = ScreenLayoutBuilder.AddColumn(columns, "Action Column", 330f);

            BuildMonsters(monsterColumn, session);
            BuildHand(handColumn, session);
            BuildRecordedActions(actionColumn, session);

            RectTransform commandRow = ScreenLayoutBuilder.AddRow(root, "Commands", 64f);
            PrimaryButton.Create(commandRow, "Resolve Combat", ResolveCombat);
            PrimaryButton.Create(commandRow, "Sample Turn", PlaySampleTurn);
            PrimaryButton.Create(commandRow, "Back To Lobby", BackToLobby);

            ScreenLayoutBuilder.AddSection(root, "Combat Log");
            ScreenLayoutBuilder.AddBody(root, logLines.Count == 0 ? "No combat log yet." : string.Join("\n", logLines));
        }

        private void BuildMonsters(Transform parent, GameSession session)
        {
            ScreenLayoutBuilder.AddSection(parent, "Monsters");
            foreach (MonsterState monster in session.Monsters)
            {
                string state = monster.IsDefeated ? "Defeated" : $"{monster.Health}/{monster.Data.MaxHealth} HP";
                ScreenLayoutBuilder.AddBody(parent, $"{monster.Data.Title}\nATK {monster.Data.AttackPower}  |  {state}");
            }
        }

        private void BuildHand(Transform parent, GameSession session)
        {
            ScreenLayoutBuilder.AddSection(parent, "Hand");
            for (int i = 0; i < session.Hand.Count; i++)
            {
                int handIndex = i;
                CardData card = session.Hand[i];
                PrimaryButton.Create(parent, $"{card.Title} ({card.Power})", () => UseCard(handIndex, card));
                ScreenLayoutBuilder.AddBody(parent, card.Description, TextAnchor.MiddleCenter);
            }
        }

        private void BuildRecordedActions(Transform parent, GameSession session)
        {
            ScreenLayoutBuilder.AddSection(parent, "Recorded Actions");
            if (session.PendingActions.Count == 0)
            {
                ScreenLayoutBuilder.AddBody(parent, "No cards recorded yet.");
                return;
            }

            foreach (CardActionRecord action in session.PendingActions)
            {
                string target = string.IsNullOrEmpty(action.TargetId) ? "self" : action.TargetId;
                ScreenLayoutBuilder.AddBody(parent, $"{action.Card.Title} -> {target}");
            }
        }

        private void UseCard(int handIndex, CardData card)
        {
            GameSession session = GameRuntime.Instance.Session;
            if (session.UseCard(handIndex, session.FirstLiveMonster()))
            {
                Render(new[] { $"{card.Title} recorded." });
            }
        }

        private void ResolveCombat()
        {
            IReadOnlyList<string> result = GameRuntime.Instance.Session.ResolveCombat();
            if (GameRuntime.Instance.Session.Phase == GamePhase.StageCleared)
            {
                RenderStageCleared(result);
                return;
            }

            Render(result);
        }

        private void PlaySampleTurn()
        {
            GameSession session = GameRuntime.Instance.Session;
            int cardsToUse = session.Hand.Count;
            for (int i = 0; i < cardsToUse; i++)
            {
                session.UseCard(0, session.FirstLiveMonster());
            }

            var result = new List<string> { $"Sample turn recorded {cardsToUse} cards." };
            result.AddRange(session.ResolveCombat());
            if (session.Phase == GamePhase.StageCleared)
            {
                RenderStageCleared(result);
                return;
            }

            Render(result);
        }

        private void RenderStageCleared(IReadOnlyList<string> result)
        {
            if (root != null)
            {
                Destroy(root.parent.gameObject);
            }

            root = ScreenLayoutBuilder.CreateScreen("Stage Cleared");
            ScreenLayoutBuilder.AddTitle(root, "Stage Cleared");
            ScreenLayoutBuilder.AddBody(root, string.Join("\n", result));
            PrimaryButton.Create(root, "Back To Lobby", BackToLobby);
        }

        private static void BackToLobby()
        {
            GameRuntime.Instance.ReturnToStageSelect();
            SceneManager.LoadScene(SceneNames.StageSelect);
        }
    }
}
