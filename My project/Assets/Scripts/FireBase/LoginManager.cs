using UnityEngine;

public class LoginManager : MonoBehaviour
{
    public enum ELoginType 
    {
        Google, FaceBook, IOS, END
    };

    private LoginPlatform platform = null;

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
            if (Result.Success)
                Debug.Log("Login Success");
            else
                Debug.Log(Result.ErrorMessage);
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
    }

    private bool Init()
    {


        return true;
    }

    #endregion
}
