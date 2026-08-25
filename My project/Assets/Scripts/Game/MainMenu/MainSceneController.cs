using UnityEngine;

public class MainSceneController : MonoBehaviour
{
    [SerializeField] SceneMoveComponent sceneMoveComponent = null;

    private void Start()
    {
        LoginManager.Instance.OnLoginSucessEvent += sceneMoveComponent.ChangeScene;
    }

}
