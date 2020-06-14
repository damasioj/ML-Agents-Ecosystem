using System;
using Unity.MLAgents;

public abstract class AgentState
{
    public abstract bool IsFinished { get; protected set; }
    public abstract void OnEnter(Agent owner); // Runs once at the beginning.
    public abstract void OnUpdate(Agent owner); // Runs every frame when state is active.
    public abstract void OnFixedUpdate(Agent owner); // Runs every fixed frame when state is active.
    public abstract void OnExit(Agent owner); // Runs once at the ending.
    public abstract void SetAction(Action action);
    public abstract void DoAction(Agent owner);
    public virtual void DoAction(Action<float[]> action, float[] vectorActions)
    {
        action(vectorActions);
        IsFinished = true;
    }
    public abstract void DoAction(Agent owner, float[] vectorActions);
}
