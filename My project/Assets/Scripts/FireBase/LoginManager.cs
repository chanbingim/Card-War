using System;
using System.Threading;
using UnityEngine;

public class LoginManager : MonoBehaviour
{
    public enum ELoginType 
    {
        Google, FaceBook, IOS, END
    };

    public event Action OnLoginSucessEvent;
    public event Action OnLoginFailEvent;

    private LoginPlatform platform = null;
    private SynchronizationContext _mainThreadContext = null;

    public void RequestLogin(ELoginType eLoginType)
    {
        if (platform != null)
        {
            platform.Release();
            platform = null;
        }

        platform = GetLoginPlatform();
        if (platform == null)
            return;

        platform.RequestLogin(eLoginType, (Result) =>
        {
            _mainThreadContext.Post(_ =>
            {
                if (Result.Success)
                {
                    Debug.Log("Login Success");
                    OnLoginSucessEvent?.Invoke();
                }
                else
                {
                    Debug.Log(Result.ErrorMessage);
                    OnLoginFailEvent?.Invoke();
                }
            }, null);
        });
    }

    private LoginPlatform GetLoginPlatform()
    {
        switch (Application.platform)
        {
            case RuntimePlatform.WindowsPlayer:
            case RuntimePlatform.WindowsEditor:
                return new WindowsLoginPlatform();

            case RuntimePlatform.Android:

                break;

            default:
                Debug.LogError(
                    $"Áö¿øÇÏÁö ¾Ê´Â ÇÃ·§Æû : {Application.platform}");
                break;
        }

        return null;
    }

    #region Default
    static public LoginManager Instance => instance;
    static private LoginManager instance = null;

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(instance.gameObject);
            instance = null;
        }

        instance = this;
        instance.Init();
    }

    private bool Init()
    {
        _mainThreadContext = SynchronizationContext.Current;

        return true;
    }

    #endregion
}
