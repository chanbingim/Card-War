using UnityEngine;
using static LoginManager;

public class LoginComponent : MonoBehaviour
{
    [SerializeField] private ELoginType eLoginType;

    public void OnClickedEvent()
    {
        LoginManager.Instance.RequestLogin(eLoginType);
    }
}
