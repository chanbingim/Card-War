using UnityEngine;
using UnityEngine.EventSystems;

public class ExitComponent : MonoBehaviour, IPointerClickHandler
{
    private async void ExitGame()
    {
        await UIManager.instance.FadeInOut(FadeCompeleted);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        ExitGame();
    }

    private void FadeCompeleted()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

}
