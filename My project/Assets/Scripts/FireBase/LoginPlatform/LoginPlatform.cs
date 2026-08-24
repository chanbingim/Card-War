using System;
using static LoginManager;

public class LoginResult
{
    public bool     Success;
    public string   IdToken;
    public string   ErrorMessage;

    public LoginResult( bool success, string idToken = null, string errorMessage = null)
    {
        Success = success;
        IdToken = idToken;
        ErrorMessage = errorMessage;
    }
}

public abstract class LoginPlatform
{
    public abstract void RequestLogin(ELoginType eLoginType, Action<LoginResult> callback);
    public abstract void Release();
}
