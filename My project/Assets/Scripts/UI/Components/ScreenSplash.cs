using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenSplash : MonoBehaviour
{
    [SerializeField] private Image flashImage;
    [SerializeField, Min(0.01f)] private float duration = 0.16f;

    [SerializeField] private float DebugStrength = 1.0f;


    [SerializeField]
    private AnimationCurve alphaCurve = new(
        new Keyframe(0.00f, 0.00f),
        new Keyframe(0.08f, 1.00f),
        new Keyframe(0.30f, 0.42f),
        new Keyframe(1.00f, 0.00f)
    );

    private Coroutine playRoutine;

    private void Awake()
    {
        SetAlpha(0f);
    }

    [ContextMenu("Play Animation")]
    private void DebugPlay()
    {
        Play(DebugStrength);
    }


    public void Play(float strength = 1f)
    {
        if (playRoutine != null)
            StopCoroutine(playRoutine);

        playRoutine = StartCoroutine(PlayRoutine(
            Mathf.Clamp01(strength)
        ));
    }

    public void Stop()
    {
        if (playRoutine != null)
            StopCoroutine(playRoutine);

        playRoutine = null;
        SetAlpha(0f);
    }

    private IEnumerator PlayRoutine(float strength)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float normalizedTime = Mathf.Clamp01(elapsed / duration);
            float alpha = alphaCurve.Evaluate(normalizedTime) * strength;

            SetAlpha(alpha);
            yield return null;

            elapsed += Time.unscaledDeltaTime;
        }

        SetAlpha(0f);
        playRoutine = null;
    }

    private void SetAlpha(float alpha)
    {
        Color color = flashImage.color;
        color.a = alpha;
        flashImage.color = color;
    }
}
