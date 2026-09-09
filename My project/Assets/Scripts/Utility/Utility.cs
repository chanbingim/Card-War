using System.Diagnostics;
using UnityEngine.UIElements;

public static class Utility
{
    private static CryptoRandom  random = new CryptoRandom();

    [Conditional("UNITY_EDITOR")]
    [DebuggerHidden]
    public static void DBG_CHECK(bool condition, string message = null)
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

    public static void FullScreen(TemplateContainer asset)
    {
        if (asset == null)
            return;

        asset.style.flexGrow = 1;
        asset.style.width = Length.Percent(100);
        asset.style.height = Length.Percent(100);
    }

    public static int GetRandomInt32(int min, int max)
    {
        return random.GetRandom(min, max);
    }

    public static float GetRandomFloat(float min, float max)
    {
        return random.GetRandom(min, max);
    }

}

