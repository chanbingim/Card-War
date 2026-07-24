using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
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

    public void Change_Scene(string level)
    {
        NextLevel = level;
        SceneManager.LoadScene("LoadingScene");
    }

    public void Change_Scene(string level, int stageIdx = 0)
    {
        NextLevel = level;
        StageIndex = stageIdx;
        SceneManager.LoadScene("LoadingScene");
    }

    public void ADD_Scene(string level)
    {
        SceneManager.LoadScene(level, LoadSceneMode.Additive);
    }

    static public GameManager   instance { get; private set; }
    static GameManager          _instance = null;
    private void Awake()
    {
        if(_instance == null)
        {
            _instance = this;
            _instance.Initialize();
            instance = _instance;

            DontDestroyOnLoad(_instance);
        }
        else
            Destroy(this);
    }

    private void Initialize()
    {

    }
}
