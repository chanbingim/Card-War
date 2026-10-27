using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteAnimation : MonoBehaviour
{
    [SerializeField] List<GachaCardAnim> _CardList;
    [SerializeField] Button              _OkButton;
    [SerializeField] GameObject          _CardTargetPoint;
    [Header("Result layout")]
    [SerializeField] RectTransform       _SingleResultPivot;
    [SerializeField] RectTransform       _TenResultPivot;
    [SerializeField] Vector2             _TenResultSpacing = new Vector2(175f, 245f);
    [SerializeField] Vector2             _SingleCardSize = new Vector2(220f, 308f);
    [SerializeField] Vector2             _TenCardSize = new Vector2(150f, 210f);

    private ChangeCanvasComponent        _OnCanvasChange = null;
    private Animator                     animator = null;

    private bool    AllVeiw = false;
    private int     GachaCount = 0;
    private int     NextCount = 0;

    private void Awake()
    {
        animator = GetComponent<Animator>();
       _OnCanvasChange = GetComponent<ChangeCanvasComponent>();
       _OnCanvasChange.enabled = false;

        _OkButton.gameObject.SetActive(false);
        _OkButton.onClick.AddListener(ClickEvent);
    }

    public void PlayAnimation(int Count) 
    {
        if (Count != 1 && Count != 10)
        {
            Debug.LogWarning($"[SpriteAnimation] Unsupported gacha count: {Count}");
            return;
        }

        gameObject.SetActive(true);

        if (animator == null)
            animator = GetComponent<Animator>();

        animator.enabled = true;
        GachaCount = Count;
        PrepareResultLayout();
        animator.SetBool("PlayAnim", false);
    }

    public void StopAnimation() 
    {
        foreach (var item in _CardList)
            item.gameObject.SetActive(false);

        animator.enabled = false;
        _OkButton.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    public void OnDestory()
    {
        _OkButton.onClick.RemoveListener(ClickEvent);
    }

    public void Update()
    {
        if(Input.anyKey && !animator.GetBool("PlayAnim"))
            NextAction();
    }

    public void NextAction()
    {
        animator.SetBool("PlayAnim", true);
        NextCount = 0;
        AllVeiw = false;
    }

    public void AnimCompelted()
    {
        NextCardAnim();
    }

    private void NextCardAnim()
    {
        var Gachasystem = GachaSystem.instance;
        if (Gachasystem == null)
        {
            Debug.Log("[GachaComponent] Not Find Gacha System");
            return;
        }

        if (_CardList.Count < GachaCount)
            return;

        if (NextCount < GachaCount)
        {
            _CardList[NextCount].gameObject.SetActive(true);
            _CardList[NextCount].Initialize(Gachasystem.RequestGachaResult(),
                                    GetCardTargetPosition(NextCount),
            () => {
                NextCardAnim();
            });
        }
        else
        {
            _OkButton.gameObject.SetActive(true);
        }
         NextCount++;
    }

    private void PrepareResultLayout()
    {
        RectTransform pivot = GachaCount == 1 ? _SingleResultPivot : _TenResultPivot;
        Vector2 cardSize = GachaCount == 1 ? _SingleCardSize : _TenCardSize;

        for (int i = 0; i < _CardList.Count; i++)
        {
            GachaCardAnim card = _CardList[i];
            card.gameObject.SetActive(false);

            RectTransform cardRect = card.transform as RectTransform;
            if (cardRect == null || pivot == null)
                continue;

            cardRect.SetParent(pivot, false);
            cardRect.anchorMin = cardRect.anchorMax = cardRect.pivot = new Vector2(0.5f, 0.5f);
            cardRect.sizeDelta = cardSize;
            cardRect.localRotation = Quaternion.identity;
            cardRect.localScale = Vector3.one;

            if (GachaCount == 1)
            {
                cardRect.anchoredPosition3D = Vector3.zero;
                continue;
            }

            int column = i % 5;
            int row = i / 5;
            cardRect.anchoredPosition3D = new Vector3(
                (column - 2) * _TenResultSpacing.x,
                (0.5f - row) * _TenResultSpacing.y,
                row * -8f);
        }
    }

    private Vector3 GetCardTargetPosition(int index)
    {
        if (index >= 0 && index < _CardList.Count)
            return _CardList[index].transform.position;

        return _CardTargetPoint != null ? _CardTargetPoint.transform.position : transform.position;
    }

    // �̰� �����غ��� CardDissolve�� �ɶ����� ī��Ʈ�ϵ�
    // Flag ������ �־�� ������ �ϰ� �ɰŰ���
    private void ClickEvent()
    {
        if (AllVeiw)
        {
            bool AllSecret = true;
            for (int i = 0; i < GachaCount && i < _CardList.Count; i++)
                _CardList[i].SetSpriteTexture();

            if (AllSecret)
            {
                _OnCanvasChange.OnPointerClick(null);
                StopAnimation();
            }
        }
        else
        {
            AllVeiw = true;
            for (int i = 0; i < GachaCount && i < _CardList.Count; i++)
                _CardList[i].OnPointerClick(null);
        }
    }
}

