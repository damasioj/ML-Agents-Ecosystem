using System;

public abstract class AgentState
{
    public abstract bool IsFinished { get; protected set; }
    public abstract void OnEnter(BasicAgent owner); // Runs once at the beginning.
    public abstract void OnUpdate(BasicAgent owner); // Runs every frame when state is active.
    public abstract void OnFixedUpdate(BasicAgent owner); // Runs every fixed frame when state is active.
    public abstract void OnExit(BasicAgent owner); // Runs once at the ending.
    public abstract void SetAction(Action action);
    public abstract void DoAction(BasicAgent owner);
    public virtual void DoAction(Action<float[]> action, float[] vectorActions)
    {
        action(vectorActions);
        IsFinished = true;
    }
    public abstract void DoAction(BasicAgent owner, float[] vectorActions);
}
