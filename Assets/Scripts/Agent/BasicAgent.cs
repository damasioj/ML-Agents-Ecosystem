using System;
using System.Collections.Generic;
using System.Linq;
using Unity.MLAgents;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// An extended abstract class based on the Unity.MLAgents.Agent class that provides basic core functions.
/// All agents must inherit from this class.
/// </summary>
public abstract class BasicAgent : Agent
{
    public event EventHandler TaskDone;

    public int maxInternalSteps;

    #region Properties
    public virtual BaseTarget Target { get; set; }
    public virtual BaseStructure Goal { get; set; }
    public Rigidbody Body { get; protected set; }
    public int InternalStepCount { get; protected set; }    
    protected bool IsDoneCalled { get; set; }
    protected Dictionary<AgentStateType, AgentState> StateDictionary { get; set; }
    private Vector3 PreviousPosition { get; set; }
    private Vector3 StartPosition { get; set; }

    private AgentStateType currentState;
    public AgentStateType CurrentState
    {
        get
        {
            return currentState;
        }
        set
        {
            if (value != currentState)
            {
                StateDictionary[currentState].OnExit(this);
                currentState = value;
                StateDictionary[currentState].OnEnter(this);
            }
        }
    }
    #endregion

    public BasicAgent() : base()
    {
        InternalStepCount = 0;
        IsDoneCalled = false;
    }

    private void Awake()
    {
        Body = GetComponent<Rigidbody>();
        CurrentState = AgentStateType.Idle;
        StartPosition = transform.position;
        PreviousPosition = StartPosition;
        AssignStateDictionary();
        OnTaskDone(); // force update of target and goal
    }

    void Update()
    {
        StateDictionary[CurrentState].OnUpdate(this);
    }

    public virtual void Reset()
    {
        IsDoneCalled = false;
        InternalStepCount = 0;
        Body.angularVelocity = Vector3.zero;
        Body.velocity = Vector3.zero;
        transform.position = StartPosition;
        PreviousPosition = StartPosition;
    }

    protected virtual void AssignStateDictionary()
    {
        StateDictionary = new Dictionary<AgentStateType, AgentState>()
        {
            [AgentStateType.Idle] = new IdleState(),
            [AgentStateType.Move] = new MoveState()
        };
    }

    public override void OnEpisodeBegin()
    {
        SetReward(0f);
        InternalStepCount = 0;
        IsDoneCalled = false;
    }

    protected virtual void Move(float[] vectorAction)
    {
        // agent is idle
        if (vectorAction[0] == 0 && vectorAction[1] == 0)
        {
            CurrentState = AgentStateType.Idle;
            return;
        }

        // agent is moving
        CurrentState = AgentStateType.Move;
        StateDictionary[CurrentState].DoAction(this, vectorAction);
    }

    protected virtual void SetDirection()
    {
        if (transform.position != PreviousPosition)
        {
            var direction = (transform.position - PreviousPosition).normalized;
            direction.y = 0;

            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 0.15F);
            PreviousPosition = transform.position;
        }
    }

    protected void OnTaskDone()
    {
        TaskDone?.Invoke(this, EventArgs.Empty);
    }

    public override void Heuristic(float[] actions)
    {
        actions[0] = Input.GetAxis("Horizontal");
        actions[1] = Input.GetAxis("Vertical");
    }

    protected void SubtractReward(float value)
    {
        AddReward(value * -1);
    }

    /// <summary>
    /// Makes the agent update their target based on the provided targets.
    /// </summary>
    /// <param name="baseTargets"></param>
    public abstract void UpdateTarget(IEnumerable<BaseTarget> baseTargets);
    
    /// <summary>
    /// Makes the agent update their goal based on the provided structures.
    /// </summary>
    /// <param name="baseStructures"></param>
    public virtual void UpdateGoal(IEnumerable<BaseStructure> baseStructures)
    {
        // when providing available goals, one could add logic here for every type of agent
        Goal = baseStructures.FirstOrDefault();
    }
}
