using System;

/// <summary>
/// Holds functions related to the agent's FSM.
/// The FSM is not actually used for the agent's AI, it's only used for executing actions and animations.
/// States can be triggered based on the agent's state or through an action given by the model.
/// </summary>
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
