using System;

/// <summary>
/// Represents a State for the manual FSM architecture.
/// These states do not use any ML.
/// </summary>
public abstract class ManualState
{
    protected ManualAgent Owner { get; set; }

    public ManualState(ManualAgent owner)
    {
        Owner = owner;
    }

    /// <summary>
    /// Determines if the state has finished executing.
    /// </summary>
    public abstract bool IsFinished { get; protected set; }
    /// <summary>
    /// Runs once when entering the state.
    /// </summary>
    public abstract void OnEnter();
    /// <summary>
    /// Runs every update when state is active.
    /// </summary>
    public abstract void OnUpdate();
    /// <summary>
    /// Runs every frame when state is active.
    /// </summary>
    public abstract void OnFixedUpdate();
    /// <summary>
    /// Runs once when leaving the state.
    /// </summary>
    public abstract void OnExit();    
    /// <summary>
    /// The SetAction allows the state logic to be abstract. The concrete subject's class can
    /// provide the action logic to be executed during the updates.
    /// </summary>
    /// <param name="action">The action to be executed during updates.</param>
    /// <param name="duration">The duration to perform the action.</param>
    public abstract void SetAction(Action action, float duration = 0f);
    public abstract void DoAction();
    public abstract void DoAction(float[] input);
}
