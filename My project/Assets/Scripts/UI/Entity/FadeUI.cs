using System;
using static Character;

public class FadeUI : UIBase
{
    Action FinishedAction = null;

    public override void Open(Object data = null)
    {
        base.Open();
        _Animator.Play_Animation();

        if(data != null)
        {
            if(FinishedAction  != null)
            {
                _Animator.OnCompleted -= FinishedAction;
                FinishedAction = (Action)data;
                _Animator.OnCompleted += FinishedAction;
            }
                
        }
    }

    public override void Close()
    {
        base.Close();
        _Animator.Pause_Animation();
        _Animator.OnCompleted -= FinishedAction;
        FinishedAction = null;
    }
}
