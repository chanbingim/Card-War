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
                _Animator._OnCompleted -= FinishedAction;

            FinishedAction = (Action)data;
            _Animator._OnCompleted += FinishedAction;
        }
    }

    public override void Close()
    {
        base.Close();
        _Animator.Pause_Animation();
        _Animator._OnCompleted -= FinishedAction;
        FinishedAction = null;
    }
}
