using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public sealed class DeckCardTransferAnimation : MonoBehaviour
{
    [SerializeField, Min(0.01f)] private float _Duration = 0.4f;
    [SerializeField, Min(0.0f)] private float _ArcHeight = 120.0f;
    [SerializeField, Range(0.1f, 1.0f)] private float _TargetScale = 0.55f;
    [SerializeField, Range(0.0f, 1.0f)] private float _FadeStartRate = 0.65f;
    [SerializeField] private Ease _MovementEase = Ease.OutCubic;

    private DeckBuildSystem DeckSystem = null;
    private bool IsSubscribed = false;

    private void OnEnable()
    {
        SubscribeEvent();
    }

    private void Start()
    {
        SubscribeEvent();
    }

    private void OnDisable()
    {
        if (DeckSystem != null && IsSubscribed)
            DeckSystem.DeckCardTransferRequested -= Play;

        DeckSystem = null;
        IsSubscribed = false;
    }

    private void SubscribeEvent()
    {
        if (IsSubscribed)
            return;

        DeckSystem = DeckBuildSystem.instance;
        if (DeckSystem == null)
            return;

        DeckSystem.DeckCardTransferRequested += Play;
        IsSubscribed = true;
    }

    private void Play(RectTransform source, RectTransform destination, Sprite cardSprite)
    {
        if (source == null || destination == null || cardSprite == null)
            return;

        Canvas rootCanvas = source.GetComponentInParent<Canvas>(true)?.rootCanvas;
        if (rootCanvas == null)
            return;

        GameObject animationObject = CreateAnimationObject(rootCanvas, source, cardSprite);
        CanvasGroup destinationGroup = GetCanvasGroup(destination);

        SetVisible(destinationGroup, false);
        StartCoroutine(PlayAfterLayout(animationObject, destination, destinationGroup));
    }

    private IEnumerator PlayAfterLayout(
        GameObject animationObject,
        RectTransform destination,
        CanvasGroup destinationGroup)
    {
        yield return null;

        Canvas.ForceUpdateCanvases();
        PlayAnimation(animationObject, destination, destinationGroup);
    }

    private static CanvasGroup GetCanvasGroup(RectTransform target)
    {
        CanvasGroup canvasGroup = target.GetComponent<CanvasGroup>();
        return canvasGroup != null
            ? canvasGroup
            : target.gameObject.AddComponent<CanvasGroup>();
    }

    private static void SetVisible(CanvasGroup canvasGroup, bool visible)
    {
        canvasGroup.alpha = visible ? 1.0f : 0.0f;
        canvasGroup.interactable = visible;
        canvasGroup.blocksRaycasts = visible;
    }

    private void PlayAnimation(
        GameObject animationObject,
        RectTransform destination,
        CanvasGroup destinationGroup)
    {
        if (animationObject == null || destination == null)
        {
            SetVisible(destinationGroup, true);
            return;
        }

        RectTransform animationRect = animationObject.GetComponent<RectTransform>();
        CanvasGroup canvasGroup = animationObject.GetComponent<CanvasGroup>();
        Canvas rootCanvas = animationRect.GetComponentInParent<Canvas>().rootCanvas;

        Vector3 startPosition = animationRect.position;
        Vector3 endPosition = destination.position;
        Vector3 middlePosition = (startPosition + endPosition) * 0.5f;
        middlePosition += Vector3.up * _ArcHeight * rootCanvas.scaleFactor;

        float duration = Mathf.Max(0.01f, _Duration);
        float fadeStartTime = duration * _FadeStartRate;
        float fadeDuration = Mathf.Max(0.01f, duration - fadeStartTime);

        Sequence sequence = DOTween.Sequence();
        sequence.SetUpdate(true);
        sequence.Append(
            animationRect
                .DOPath(
                    new[] { startPosition, middlePosition, endPosition },
                    duration,
                    PathType.CatmullRom,
                    PathMode.Ignore)
                .SetEase(_MovementEase));
        sequence.Join(
            animationRect
                .DOScale(_TargetScale, duration)
                .SetEase(Ease.InQuad));
        sequence.Insert(
            fadeStartTime,
            canvasGroup.DOFade(0.0f, fadeDuration));
        sequence.SetLink(gameObject, LinkBehaviour.KillOnDestroy);
        bool isFinished = false;
        void Finish()
        {
            if (isFinished)
                return;

            isFinished = true;
            SetVisible(destinationGroup, true);

            if (animationObject != null)
                Destroy(animationObject);
        }

        sequence.OnComplete(Finish);
        sequence.OnKill(Finish);
    }

    private static GameObject CreateAnimationObject(
        Canvas rootCanvas,
        RectTransform source,
        Sprite cardSprite)
    {
        GameObject animationObject = new GameObject(
            "DeckCardTransfer",
            typeof(RectTransform),
            typeof(CanvasRenderer),
            typeof(Image),
            typeof(CanvasGroup));

        RectTransform animationRect = animationObject.GetComponent<RectTransform>();
        animationRect.SetParent(rootCanvas.transform, false);
        animationRect.position = source.position;
        animationRect.rotation = source.rotation;
        animationRect.sizeDelta = source.rect.size;
        animationRect.SetAsLastSibling();

        Image image = animationObject.GetComponent<Image>();
        image.sprite = cardSprite;
        image.preserveAspect = true;
        image.raycastTarget = false;

        CanvasGroup canvasGroup = animationObject.GetComponent<CanvasGroup>();
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;

        return animationObject;
    }
}
