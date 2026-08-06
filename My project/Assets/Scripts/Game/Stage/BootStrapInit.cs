using Cysharp.Threading.Tasks;
using System.Collections;
using UnityEngine;

public class BootStrapInit : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private async void Start()
    {
        await InitAsync();
    }


    private async UniTask InitAsync()
    {
        await AddressableManager.instance.InitializeAsync();
        await UniTask.WhenAll(
            UIManager.instance.InitializeAsync(),
            DataManager.instance.InitializeAsync()
         );

        GameManager.instance.ChangeScene("MainMenu");

        Debug.Log("비동기 2초 대기");
        await UniTask.Delay(2000);
        Debug.Log("비동기 초기화 완료");

    }
}
