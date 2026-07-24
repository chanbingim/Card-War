using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

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

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(GameMgr.GetNextLevel());
        asyncOperation.allowSceneActivation = false; //로딩이 완료되는대로 씬을 활성화할것인지

        float ProgressValue = 1f;
        bool IsAddressableLoad = true;
        int StageIdx = GameMgr.StageIndex;

        if(StageIdx != 0)
        {
            ProgressValue = 0f;
            IsAddressableLoad = false;
            var addressableTask = AddressableManager.instance.LoadLabelAll($"Stage{StageIdx}", value =>
            {
                ProgressValue = value;
                if (value >= 1f)
                {
                    IsAddressableLoad = true;
                }
            });
        }

        yield return new WaitForSeconds(1f);

        while (!IsAddressableLoad)
        {
            _text.text = "패키지 로딩중~~~";
            _Progress.value = ProgressValue * 0.5f;
            yield return null;
        }

        yield return new WaitForSeconds(1f);
        while (!asyncOperation.isDone) //isDone는 로딩이 완료되었는지 확인하는 변수
        {
            _text.text = "화면 구성 하는중";

            float sceneProgress = asyncOperation.progress / 0.9f;
            float totalProgress = sceneProgress * 0.5f + ProgressValue * 0.5f;
            _Progress.value = totalProgress;

            if (asyncOperation.progress >= 0.9f && ProgressValue >= 1f)
            {
                asyncOperation.allowSceneActivation = true; //씬 활성화
            }

            yield return null;
        }
    }
}
