using System.Collections.Generic;

public class ActionQueue
{
    public  Queue<CharacterAction> _ActQueues { get; private set; } = new Queue<CharacterAction>();
    public  Queue<CharacterAction> _OldActQueues { get; private set; } = new Queue<CharacterAction>();
    private CharacterAction _CurAction;

    public void ADD_ActQueue(CharacterAction Act)
    {
        _ActQueues.Enqueue(Act);
    }

    public void Update_PlayerAction()
    {
        if (_CurAction == null)
            Next_Action();

    }

    public void Next_Action()
    {
        if (_CurAction != null)
        {
            _OldActQueues.Enqueue(_CurAction);
            if (_ActQueues.Count > 0)
            {
                _CurAction = _ActQueues.Dequeue();
            }
            else
            {
                _CurAction = null;
            }
        }
        else
        {
            if (_ActQueues.Count > 0)
            {
                _CurAction = _ActQueues.Dequeue();
            }
        }
    }
}
