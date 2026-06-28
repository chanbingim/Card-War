using DG.Tweening;
using UnityEngine;

public class UIBase : MonoBehaviour
{
    [SerializeField] bool bIsPopup = false;
    DoTweenAnimator       _Animator;

    private void Start()
    {
        _Animator = GetComponent<DoTweenAnimator>();
    }

    public void Open()
    {
        gameObject.SetActive(true);

        if(bIsPopup)
            transform.SetAsLastSibling();
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        transform.DOKill();
    }

    private void OnDestroy()
    {
        transform.DOKill();
    }
}
