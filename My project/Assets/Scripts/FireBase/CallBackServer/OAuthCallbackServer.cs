using System;
using System.Net;
using System.Text;
using System.Threading;
using UnityEngine;

public class OAuthCallbackServer
{
    private HttpListener _listener;
    private Thread _thread;

    private Action<LoginResult> _onCallback;

    public void Start(int port, Action<LoginResult> onCallback)
    {
        _onCallback = onCallback;
        _listener = new HttpListener();

        _listener.Prefixes.Add(
            $"http://127.0.0.1:{port}/"
        );

        _listener.Start();

        Debug.Log(
            $"OAuth Server Start : http://127.0.0.1:{port}/"
        );

        _thread = new Thread(Listen);
        _thread.Start();
    }

    private void Listen()
    {
        try
        {
            // 브라우저가 callback을 요청할 때까지 대기
            HttpListenerContext context = _listener.GetContext();
            string path = context.Request.Url.AbsolutePath;
            string query = context.Request.Url.Query;

            Debug.Log($"Callback Path : {path}");
            Debug.Log($"Callback Query : {query}");

            // 브라우저에 보여줄 응답
            string html =
                "<html>" +
                "<body>" +
                "<h2>Login Complete</h2>" +
                "<p>You can close this window.</p>" +
                "</body>" +
                "</html>";

            byte[] buffer =
                Encoding.UTF8.GetBytes(html);

            context.Response.ContentType =
                "text/html";

            context.Response.ContentLength64 =
                buffer.Length;

            context.Response.OutputStream.Write(
                buffer,
                0,
                buffer.Length
            );

            string successString = context.Request.QueryString["success"];

            LoginResult result = new LoginResult(bool.Parse(successString),
                                                context.Request.QueryString["token"],
                                                context.Request.QueryString["error"]);

            context.Response.OutputStream.Close();
            // Unity 쪽으로 결과 전달
            _onCallback?.Invoke(result);
        }
        catch (Exception e)
        {
            Debug.LogError(
                $"OAuth Server Error : {e}"
            );
        }
    }

    public void Stop()
    {
        try
        {
            if (_listener != null)
            {
                if (_listener.IsListening)
                    _listener.Stop();

                _listener.Close();
                _listener = null;
            }

            if (_thread != null)
            {
                if (_thread.IsAlive)
                    _thread.Join(1000);

                _thread = null;
            }
        }
        catch (Exception e)
        {
            Debug.LogError(
                $"OAuth Server Stop Error : {e}");
        }
    }
}