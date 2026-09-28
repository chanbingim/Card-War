using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SpriteAnimation : MonoBehaviour
{
    [SerializeField] List<GachaCardAnim> _CardList;
    [SerializeField] Button              _OkButton;
    [SerializeField] GameObject          _CardTargetPoint;

    private ChangeCanvasComponent        _OnCanvasChange = null;
    private Animator                     animator = null;
    private bool    AllVeiw = false;
    private int     NextCount = 0;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        _OnCanvasChange = GetComponent<ChangeCanvasComponent>();
        _OnCanvasChange.enabled = false;

        _OkButton.gameObject.SetActive(false);
        _OkButton.onClick.AddListener(ClickEvent);
    }

    public void PlayAnimation() 
    {
        gameObject.SetActive(true);

        if (animator == null)
            animator = GetComponent<Animator>();

        animator.enabled = true;
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

        if(NextCount < 1)
        {
            _CardList[NextCount].gameObject.SetActive(true);
            _CardList[NextCount].Initialize(Gachasystem.RequestGachaResult(),
                                    _CardTargetPoint.transform.position,
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

    private void ClickEvent()
    {
        if (AllVeiw)
        {
            bool AllSecret = true;
            foreach (var item in _CardList)
                item.SetSpriteTexture();

            if (AllSecret)
            {
                _OnCanvasChange.OnPointerClick(null);
                StopAnimation();
            }
        }
        else
        {
            AllVeiw = true;
            foreach (var item in _CardList)
                item.OnPointerClick(null);
        }
    }
}
