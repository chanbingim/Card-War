using DG.Tweening;
using UnityEngine;

public class UIBase : MonoBehaviour
{
    [SerializeField] bool bIsPopup = false;
    protected DoTweenAnimator       _Animator;

    private void Awake()
    {
        _Animator = GetComponent<DoTweenAnimator>();
        if(_Animator != null )
            _Animator.Initialize();
    }

    public virtual void Open(System.Object data = null)
    {
        gameObject.SetActive(true);

        if(bIsPopup)
            transform.SetAsLastSibling();
    }

    public virtual void Close()
    {
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        transform.DOKill();
    }

    protected virtual void OnDestroy()
    {
        transform.DOKill();
    }

    private void OnEnable()
    {
        if(bIsPopup)
            transform.localScale = new Vector3(0, 0, 0);
    }
}
