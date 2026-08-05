using System;
using System.Collections.Generic;

public class ActionQueue
{
    public  Queue<CardAction> _ActQueues { get; private set; } = new Queue<CardAction>();
    public  Queue<CardAction> _OldActQueues { get; private set; } = new Queue<CardAction>();
    private CardAction _CurAction;

    public void ADD_ActQueue(CardAction Act)
    {
        _ActQueues.Enqueue(Act);
    }

    public void Next_Action()
    {
        if (_CurAction != null)
        {
            _OldActQueues.Enqueue(_CurAction);
            _CurAction = null;
        }

        if (_ActQueues.Count > 0)
        {
            _CurAction = _ActQueues.Dequeue();
            EventBus.Publish<CardActionEvent>(new CardActionEvent(_CurAction));
        }
    }

    public void Update_PlayerAction()
    {
        if (_CurAction != null)
        {
            _CurAction.ActObject.Update_Action(_CurAction);
        }
    }
}
