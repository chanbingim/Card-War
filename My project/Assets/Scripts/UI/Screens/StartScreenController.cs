using TurnCardGame.Game;
using TurnCardGame.UI.Components;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TurnCardGame.UI.Screens
{
    public sealed class StartScreenController : MonoBehaviour
    {
        private void Awake()
        {
            RectTransform root = ScreenLayoutBuilder.CreateScreen("Start");
            ScreenLayoutBuilder.AddTitle(root, "Turn Card Game");
            ScreenLayoutBuilder.AddBody(root, "Draw cards, record actions, and resolve each turn against the stage monster.");
            ScreenLayoutBuilder.AddBody(root, "Press start to move to the stage lobby.");
            PrimaryButton.Create(root, "Start Game", StartGame);
        }

        private void StartGame()
        {
            GameRuntime.Instance.BeginNewGame();
            SceneManager.LoadScene(SceneNames.StageSelect);
        }
    }
}
