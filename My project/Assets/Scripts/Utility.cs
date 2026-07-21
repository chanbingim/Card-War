using System.Diagnostics;

public static class Utility
{
    [Conditional("UNITY_EDITOR")]
    [DebuggerHidden]
    public static void DBG_CHECK(bool condition, string? message = null)
    {
        if (!condition)
        {
            UnityEngine.Debug.LogError(message);
        }
    }

    [DebuggerHidden]
    public static bool CHECK<T>(T x)
    {
        if(x == null)
        {
            UnityEngine.Debug.LogWarning("Object is Null");
            return false;
        }

        return true;
    }
}

