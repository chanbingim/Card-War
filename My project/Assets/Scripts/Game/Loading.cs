using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;

public class Loading : MonoBehaviour
{
    [SerializeField] private Slider _Progress = null;
    [SerializeField] private Text _text = null;

    private async void Start()
    {
        await LoadingAsync();
    }

    private async UniTask LoadingAsync()
    {
        var GameMgr = GameManager.instance;
        string LoadAssetGroupName = GameMgr.GetNextLevel();

        int StageIdx = GameMgr.StageIndex;
        if (StageIdx != 0)
            LoadAssetGroupName = $"Stage{StageIdx}";

        await LoadingStageAddressable(LoadAssetGroupName);
        await LoadingStageObjectPool(LoadAssetGroupName);
        await LoadNextScene();
    }

    private async UniTask LoadingStageAddressable(string LoadAssetGroupName)
    {
        var GameMgr = GameManager.instance;
        float ProgressValue = 0f;

        UniTask addressableTask;
        addressableTask = AddressableManager.instance.LoadLabelAll(LoadAssetGroupName, LoadAssetGroupName,
               value =>
               {
                   ProgressValue = value;
               });

        _text.text = "패키지 로딩중~~~";
        while (!addressableTask.Status.IsCompleted())
        {
            _Progress.value = ProgressValue * 0.2f;
            await UniTask.Yield();
        }
    }

    private async UniTask LoadingStageObjectPool(string LoadAssetGroupName)
    {
        _text.text = "풀 매니저 로딩중~~~";
        var PoolMgr = PoolManager.Instance;

        PoolMgr.Release();
        UniTask PoolManagerTask = PoolMgr.InitializeAsync(LoadAssetGroupName);

        while (!PoolManagerTask.Status.IsCompleted())
        {
            _Progress.value += 0.3f;
            await UniTask.Yield();
        }

        await UniTask.Delay(2000);
    }

    private async UniTask LoadNextScene()
    {
        float fProgress = _Progress.value;
        var GameMgr = GameManager.instance;
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(GameMgr.GetNextLevel(), LoadSceneMode.Additive);
        loadOperation.allowSceneActivation = false; //로딩이 완료되는대로 씬을 활성화할것인지

        _text.text = "화면 구성 하는중";
        while (!loadOperation.isDone) //isDone는 로딩이 완료되었는지 확인하는 변수
        {
            float sceneProgress = loadOperation.progress / 0.9f;
            _Progress.value = fProgress + sceneProgress * 0.5f;

            if (loadOperation.progress >= 0.9f)
            {
                loadOperation.allowSceneActivation = true; //씬 활성화
            }
            await UniTask.Yield();
        }

        Scene nextScene = SceneManager.GetSceneByName(GameMgr.GetNextLevel());
        if (nextScene.IsValid() && nextScene.isLoaded)
        {
            SceneManager.SetActiveScene(nextScene);
            PoolManager.Instance.PoolRootMoveScene(nextScene);
        }

        Debug.Log(SceneManager.GetActiveScene().name);

        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Debug.Log(SceneManager.GetSceneAt(i).name);
        }

        await SceneManager.UnloadSceneAsync("LoadingScene");
    }
}
