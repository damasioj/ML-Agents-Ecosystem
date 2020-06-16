using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;
using UnityEngine.Events;

public abstract class BasicAgent : Agent
{
    [HideInInspector] public UnityEvent<BasicAgent> onTaskDone;

    public int maxInternalSteps;
    private Vector3 previousPosition;

    #region Properties
    [HideInInspector] public Rigidbody Body { get; protected set; }
    [HideInInspector] public int InternalStepCount { get; protected set; }    
    protected bool IsDoneCalled { get; set; }
    protected Dictionary<AgentStateType, AgentState> StateDictionary { get; set; }
    public virtual BaseTarget Target { get; set; }
    public virtual BaseStructure Goal { get; set; }

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

    public BasicAgent()
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
        if (transform.position != previousPosition)
        {
            var direction = (transform.position - previousPosition).normalized;
            direction.y = 0;

            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 0.15F);
            previousPosition = transform.position;
        }
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

    public abstract void UpdateTarget(IEnumerable<BaseTarget> baseTargets);
}
