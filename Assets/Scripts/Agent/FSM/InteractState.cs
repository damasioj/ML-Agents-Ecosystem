using System;

public class InteractState : AgentState
{
    private Action actionToExecute;
    private float counter = 0f;

    public override bool IsFinished { get; protected set; }

    public override void SetAction(Action action)
    {
        actionToExecute = action;
    }

    public override void DoAction(BasicAgent owner)
    {
        return;
    }

    public override void DoAction(BasicAgent owner, float[] vectorActions)
    {
        return;
    }

    public override void OnEnter(BasicAgent owner)
    {
        IsFinished = false;
        counter = owner.StepCount;
        // todo : start animation
    }

    public override void OnExit(BasicAgent owner)
    {
        return;
    }

    public override void OnFixedUpdate(BasicAgent owner)
    {
        if (owner.StepCount - counter >= 50)
        {
            actionToExecute();
            IsFinished = true;
            owner.CurrentState = AgentStateType.Idle;
        };
    }

    public override void OnUpdate(BasicAgent owner)
    {
        return;
    }
}
