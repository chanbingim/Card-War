using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public  string  GetNextLevel() { return NextLevel; }
    private string  NextLevel = "";

    public void GameExit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }
    public void Chanage_Level(string level)
    {
        NextLevel = level;
        SceneManager.LoadScene(NextLevel);
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
