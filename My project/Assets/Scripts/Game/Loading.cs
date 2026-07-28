using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;
using System.Threading.Tasks;

public class Loading : MonoBehaviour
{
    [SerializeField] private Slider _Progress = null;
    [SerializeField] private Text   _text = null;

    private void Start()
    {
        StartCoroutine(LoadingAsync());
    }

    IEnumerator LoadingAsync()
    {
        var GameMgr = GameManager.instance;

        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(GameMgr.GetNextLevel(), LoadSceneMode.Additive);
        loadOperation.allowSceneActivation = false; //로딩이 완료되는대로 씬을 활성화할것인지

        float ProgressValue = 1f;
        int StageIdx = GameMgr.StageIndex;

        Task addressableTask = null;
        if (StageIdx != 0)
        {
            ProgressValue = 0f;
            addressableTask = AddressableManager.instance.LoadLabelAll($"Stage{StageIdx}", value =>
            {
                ProgressValue = value;
            });
        }
        else
        {
            addressableTask = AddressableManager.instance.InitializeAsync(value =>
            {
                ProgressValue = value;
            });
        }

        _text.text = "패키지 로딩중~~~";
        while (!addressableTask.IsCompleted)
        {
            _Progress.value = ProgressValue * 0.5f;
            yield return null;
        }

        yield return new WaitForSeconds(1f);
    
        _text.text = "화면 구성 하는중";
        while (!loadOperation.isDone) //isDone는 로딩이 완료되었는지 확인하는 변수
        {
            float sceneProgress = loadOperation.progress / 0.9f;
            float totalProgress = sceneProgress * 0.5f + ProgressValue * 0.5f;
            _Progress.value = totalProgress;

            if (loadOperation.progress >= 0.9f && ProgressValue >= 1f)
            {
                loadOperation.allowSceneActivation = true; //씬 활성화
                
            }

            yield return null;
        }
        yield return loadOperation;
        Scene nextScene = SceneManager.GetSceneByName(GameMgr.GetNextLevel());

        if (nextScene.IsValid() && nextScene.isLoaded)
        {
            SceneManager.SetActiveScene(nextScene);
        }

        Debug.Log(SceneManager.GetActiveScene().name);

        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Debug.Log(SceneManager.GetSceneAt(i).name);
        }
        yield return SceneManager.UnloadSceneAsync("LoadingScene");

        
    }
}
