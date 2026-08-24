using System;
using UnityEngine;
using static LoginManager;

public class WindowsLoginPlatform: LoginPlatform
{
    private OAuthCallbackServer _server;
    private const int Port = 5000;

    public override void RequestLogin(ELoginType eLoginType, Action<LoginResult> callback)
    {
        if(_server != null)
        {
            _server.Stop();
        }

        _server = new OAuthCallbackServer();
        // 1. localhost 서버 시작
        _server.Start(Port, result =>
            {
                callback?.Invoke(result);
                _server.Stop();
            });


        // 2. Web Login 페이지 실행
        string url = "http://localhost:8000/UnityWebLogin.html";
        Application.OpenURL(url);
    }

    public override void Release()
    {
        if (_server != null)
        {
            _server.Stop();
        }
    }
}
