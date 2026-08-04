using UnityEngine;

public class FadeUI : UIBase
{
    public override void Open(System.Object data = null)
    {
        base.Open();
        _Animator.Play_Animation();
    }

    public override void Close()
    {
        base.Close();
        _Animator.Pause_Animation();
    }
}
