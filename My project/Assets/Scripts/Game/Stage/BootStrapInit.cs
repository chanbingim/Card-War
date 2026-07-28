using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class BootStrapInit : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(Init());

        Debug.Log("BootStrapInit Á¾·á");
    }

    private void Awake()
    {
     
    }


    IEnumerator Init()
    {
        Task task = AddressableManager.instance.InitializeAsync();
        while (!task.IsCompleted)
            yield return null;

        DataManager.instance.Initialize();
        task = UIManager.instance.Initialize();
        while (!task.IsCompleted)
            yield return null;

        GameManager.instance.ChangeScene("MainMenu");
        Debug.Log("Coroutine");
    }
}
