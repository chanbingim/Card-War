using UnityEngine;

public class FadeUI : UIBase
{
    public override void Open()
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
