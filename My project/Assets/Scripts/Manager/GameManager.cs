using System.Collections;
using System.Collections.Generic;
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
    public void Change_Scene(string level)
    {
        NextLevel = level;
        SceneManager.LoadScene(NextLevel);
    }

    public void ADD_Scene(string level)
    {
        NextLevel = level;
        SceneManager.LoadScene(NextLevel, LoadSceneMode.Additive);
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
