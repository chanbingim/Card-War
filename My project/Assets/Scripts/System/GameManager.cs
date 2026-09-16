using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameMode
{
    SinglePlayer,
    Multiplayer
}

public class GameManager : MonoBehaviour
{
    public GameMode EGameMode { get; private set; } = GameMode.SinglePlayer;

    public  int     StageIndex { get; private set; }
    private string  NextLevel = "";

    public  string  GetNextLevel() { return NextLevel; }
    public void GameExit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }

    public void ChangeScene(string level, int stageIdx = 0)
    {
        StartCoroutine(ChangeSceneRoutine(level, stageIdx));
    }

    private IEnumerator ChangeSceneRoutine(string level, int stageIdx)
    {
        if (string.IsNullOrEmpty(NextLevel) == false)
            yield return SceneManager.UnloadSceneAsync(NextLevel);

        yield return SceneManager.LoadSceneAsync("LoadingScene", LoadSceneMode.Additive);

        NextLevel = level;
        StageIndex = stageIdx;
    }

    static public GameManager   instance { get; private set; }
    static GameManager          _instance = null;
    private void Awake()
    {
        if(_instance == null)
        {
            _instance = this;
            instance = _instance;
            _instance.Initialize();

            DontDestroyOnLoad(_instance);
        }
        else
            Destroy(this);
    }

    private void Initialize()
    {

    }
}
