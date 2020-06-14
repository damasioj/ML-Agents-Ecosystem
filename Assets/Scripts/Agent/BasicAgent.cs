using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;

public class BasicAgent : Agent
{
    public int maxInternalSteps;    

    #region Properties
    [HideInInspector] public Rigidbody RigidBody { get; private set; }
    [HideInInspector] public int InternalStepCount { get; private set; }
    protected bool IsDoneCalled { get; set; }
    protected Dictionary<State, AgentState> StateDictionary { get; set; }
    public virtual BaseSource Target { get; set; }
    public virtual BaseStructure Goal { get; set; }

    private State currentState;
    public State CurrentState
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

    void Start()
    {
        InternalStepCount = 0;
        IsDoneCalled = false;
    }

    void Update()
    {
        StateDictionary[CurrentState].OnUpdate(this);
    }

    protected virtual void AssignStateDictionary()
    {
        StateDictionary = new Dictionary<State, AgentState>()
        {
            [State.Idle] = new IdleState(),
            [State.Move] = new MoveState()
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
            CurrentState = State.Idle;
            return;
        }

        // agent is moving
        CurrentState = State.Move;
        StateDictionary[CurrentState].DoAction(this, vectorAction);
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
}
