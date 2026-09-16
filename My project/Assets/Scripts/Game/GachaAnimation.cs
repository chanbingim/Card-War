using System.Collections.Generic;
using UnityEngine;

public class SpriteAnimation : MonoBehaviour
{
    [SerializeField] List<GachaCardAnim> _CardList;
    [SerializeField] GameObject         _OkButton;

    [SerializeField] GameObject          _CardTargetPoint;
    private Animator animator = null;
    private int NextCount = 0;


    private void Awake()
    {
        animator = GetComponent<Animator>();
        gameObject.SetActive(false);
        _OkButton.SetActive(false);
    }

    public void PlayAnimation() 
    {
        gameObject.SetActive(true);
        animator.enabled = true;
        animator.SetBool("PlayAnim", false);
    }
    public void StopAnimation() 
    {
        foreach (var item in _CardList)
            item.gameObject.SetActive(false);

        animator.enabled = false;
        _OkButton.SetActive(false);
        gameObject.SetActive(false);
    }

    public void Update()
    {
        if(Input.anyKey)
            NextAction();
    }

    public void NextAction()
    {
        animator.SetBool("PlayAnim", true);
        NextCount = 0;
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
}
