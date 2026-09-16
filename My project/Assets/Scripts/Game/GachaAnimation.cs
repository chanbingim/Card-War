using UnityEngine;
using System.Collections.Generic;

public class SpriteAnimation : MonoBehaviour
{
    [SerializeField] List<GachaCardAnim> _CardList;
    private Animator animator = null;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        gameObject.SetActive(false);
    }

    public void PlayAnimation() 
    {
        gameObject.SetActive(true);
        animator.enabled = true;
        animator.SetBool("PlayAnim", false);
    }
    public void StopAnimation() 
    {
        animator.enabled = false;
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
    }
}
