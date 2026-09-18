using System;
using static Character;

public class FadeUIDesc
{
    public FadeUIDesc(bool bReverse = false, Action onCompelted = null)
    {
        IsReverse = bReverse;
        OnCompelted = onCompelted;
    }

    public bool IsReverse;
    public Action OnCompelted;
}

public class FadeUI : UIBase
{
    Action FinishedAction = null;

    public void OnEnable()
    {
        _Animator.OnCompleted += OnFinishedAction;
    }

    protected override void OnDisable()
    {
        _Animator.OnCompleted -= OnFinishedAction;
    }

    public override void Open(Object data = null)
    {
        base.Open();

        var Desc = data as FadeUIDesc;
        _Animator.Play_Animation(Desc.IsReverse);

        if (Desc != null)
        {
            if (Desc.OnCompelted != null)
            {
                FinishedAction = Desc.OnCompelted;
            }
        }
    }

    private void OnFinishedAction()
    {
        FinishedAction?.Invoke();
        Close();
    }

    public override void Close()
    {
        base.Close();
        _Animator.Pause_Animation();
        FinishedAction = null;
    }
}
