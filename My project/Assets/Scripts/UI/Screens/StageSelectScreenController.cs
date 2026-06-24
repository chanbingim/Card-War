using TurnCardGame.Data;
using TurnCardGame.Game;
using TurnCardGame.UI.Components;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TurnCardGame.UI.Screens
{
    public sealed class StageSelectScreenController : MonoBehaviour
    {
        private void Awake()
        {
            GameRuntime runtime = GameRuntime.Instance;
            if (runtime.Session.Phase == GamePhase.Start)
            {
                runtime.BeginNewGame();
            }

            RectTransform root = ScreenLayoutBuilder.CreateScreen("Stage Select");
            ScreenLayoutBuilder.AddTitle(root, "Stage Select");
            ScreenLayoutBuilder.AddBody(root, $"Player HP: {runtime.Session.PlayerHealth} / 30");
            ScreenLayoutBuilder.AddBody(root, $"Available Stages: {runtime.Stages.Count}");
            ScreenLayoutBuilder.AddSection(root, "Choose Stage");

            foreach (StageData stage in runtime.Stages)
            {
                StageData selectedStage = stage;
                string label = $"{selectedStage.Title}  |  Monsters {selectedStage.Monsters.Count}";
                PrimaryButton.Create(root, label, () => SelectStage(selectedStage));
            }
        }

        private static void SelectStage(StageData stage)
        {
            GameRuntime.Instance.SelectStage(stage);
            SceneManager.LoadScene(SceneNames.Battle);
        }
    }
}
