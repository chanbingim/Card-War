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
            Debug.Log("================================");
            Debug.Log("OAuth Server Listening...");
            Debug.Log("================================");


            // ========================================================
            // Callback 대기
            // ========================================================

            HttpListenerContext context =
                _listener.GetContext();


            Debug.Log("OAuth Callback Received");


            // ========================================================
            // URL
            // ========================================================

            string path =
                context.Request.Url.AbsolutePath;

            string query =
                context.Request.Url.Query;


            Debug.Log(
                $"Callback Path : {path}"
            );

            Debug.Log(
                $"Callback Query : {query}"
            );

            Debug.Log(
                $"OAuth Request : {context.Request.Url}"
            );


            // ========================================================
            // Query
            // ========================================================

            string successString =
                context.Request.QueryString["success"];

            string token =
                context.Request.QueryString["token"];

            string error =
                context.Request.QueryString["error"];


            Debug.Log(
                $"Success String : " +
                $"{successString ?? "NULL"}"
            );

            Debug.Log(
                $"Token : " +
                $"{(string.IsNullOrEmpty(token) ? "NULL" : "EXISTS")}"
            );

            Debug.Log(
                $"Token Length : " +
                $"{(token == null ? 0 : token.Length)}"
            );

            Debug.Log(
                $"Error : " +
                $"{error ?? "NULL"}"
            );


            // ========================================================
            // success 검사
            // ========================================================

            if (string.IsNullOrEmpty(successString))
            {
                Debug.LogError(
                    "OAuth Callback Error : " +
                    "success parameter is NULL"
                );


                string html =
                    "<html>" +
                    "<body>" +
                    "<h2>Login Failed</h2>" +
                    "<p>success parameter is missing.</p>" +
                    "</body>" +
                    "</html>";


                byte[] buffer =
                    Encoding.UTF8.GetBytes(html);


                context.Response.StatusCode = 400;

                context.Response.ContentType =
                    "text/html; charset=utf-8";

                context.Response.ContentLength64 =
                    buffer.Length;


                context.Response.OutputStream.Write(
                    buffer,
                    0,
                    buffer.Length
                );


                context.Response.OutputStream.Close();


                return;
            }


            // ========================================================
            // bool 변환
            // ========================================================

            if (!bool.TryParse(
                    successString,
                    out bool success))
            {
                Debug.LogError(
                    $"Invalid success value : {successString}"
                );


                string html =
                    "<html>" +
                    "<body>" +
                    "<h2>Login Failed</h2>" +
                    "<p>Invalid success parameter.</p>" +
                    "</body>" +
                    "</html>";


                byte[] buffer =
                    Encoding.UTF8.GetBytes(html);


                context.Response.StatusCode = 400;

                context.Response.ContentType =
                    "text/html; charset=utf-8";

                context.Response.ContentLength64 =
                    buffer.Length;


                context.Response.OutputStream.Write(
                    buffer,
                    0,
                    buffer.Length
                );


                context.Response.OutputStream.Close();


                return;
            }


            // ========================================================
            // LoginResult
            // ========================================================

            LoginResult result =
                new LoginResult(
                    success,
                    token,
                    error
                );


            Debug.Log(
                $"LoginResult Created : " +
                $"Success = {success}"
            );


            // ========================================================
            // Browser Response
            // ========================================================

            string responseHtml =
                success

                ?

                "<html>" +
                "<head>" +
                "<meta charset='UTF-8'>" +
                "</head>" +
                "<body>" +
                "<h2>Login Complete</h2>" +
                "<p>You can close this window.</p>" +
                "</body>" +
                "</html>"

                :

                "<html>" +
                "<head>" +
                "<meta charset='UTF-8'>" +
                "</head>" +
                "<body>" +
                "<h2>Login Failed</h2>" +
                "<p>" +
                (error ?? "Unknown error") +
                "</p>" +
                "</body>" +
                "</html>";


            byte[] responseBuffer =
                Encoding.UTF8.GetBytes(
                    responseHtml
                );


            context.Response.StatusCode = 200;

            context.Response.ContentType =
                "text/html; charset=utf-8";

            context.Response.ContentEncoding =
                Encoding.UTF8;

            context.Response.ContentLength64 =
                responseBuffer.Length;


            context.Response.OutputStream.Write(
                responseBuffer,
                0,
                responseBuffer.Length
            );


            context.Response.OutputStream.Flush();

            context.Response.OutputStream.Close();


            // ========================================================
            // Unity Callback
            // ========================================================

            Debug.Log(
                "Invoking Unity Callback..."
            );


            _onCallback?.Invoke(result);


            Debug.Log(
                "Unity Callback Finished"
            );
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