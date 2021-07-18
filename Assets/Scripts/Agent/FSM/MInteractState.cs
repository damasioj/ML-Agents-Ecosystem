using System;

public class MInteractState : ManualState
{
    private Action actionToExecute;
    private float counter = 0f;
    private float actionDuration = 0f;

    public override bool IsFinished { get; protected set; }

    public MInteractState(ManualAgent owner)
        : base(owner) { }

    public override void SetAction(Action action, float duration = 0f)
    {
        actionDuration = duration;
        counter = 0f;
        actionToExecute = action;
    }

    public override void DoAction()
    {
        return;
    }

    public override void DoAction(float[] vectorActions)
    {
        return;
    }

    public override void OnEnter()
    {
        IsFinished = false;
    }

    public override void OnExit()
    {
        return;
    }

    public override void OnFixedUpdate()
    {
        if (!IsFinished && counter >= actionDuration)
        {
            actionToExecute();
            IsFinished = true;
            Owner.CurrentState = AgentStateType.Idle;
        }

        counter++;
    }

    public override void OnUpdate()
    {
        return;
    }
}
