using System.Collections.Generic;
using System.Linq;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class CollectorAgent : BasicAgent
{    
    private BaseResource resource;
    private bool HasResource => resource is object;
    private bool IsAtResource { get; set; }
    new public BaseSource Target { get; set; }

    protected override void AssignStateDictionary()
    {
        StateDictionary = new Dictionary<AgentStateType, AgentState>()
        {
            [AgentStateType.Idle] = new IdleState(),
            [AgentStateType.Move] = new MoveState(),
            [AgentStateType.Interact] = new InteractState()
        };
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case "goal":
                if (HasResource && other.gameObject == Goal.gameObject)
                {
                    AddReward(0.5f);
                    var deposit = other.gameObject.GetComponent(typeof(BaseStructure)) as BaseStructure;
                    deposit.AddResource(ref resource);
                    ValidateJobComplete();
                    ValidateGoalComplete();
                    InternalStepCount = 0;
                    Debug.Log($"Current Reward: {GetCumulativeReward()}");
                }
                break;
            case "target":
                if (!HasResource && other.GetComponent<BaseSource>().Equals(Target))
                {
                    IsAtResource = true;
                }
                break;
            case "boundary":
                if (!IsDoneCalled)
                {
                    IsDoneCalled = true;
                    SubtractReward(0.1f);
                    Debug.Log($"Current Reward: {GetCumulativeReward()}");
                }
                break;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("target"))
        {
            IsAtResource = false;
        }
    }

    public override void OnEpisodeBegin()
    {
        base.OnEpisodeBegin();
        
        if (!Goal.IsComplete)
        {
            Body.angularVelocity = Vector3.zero;
            Body.velocity = Vector3.zero;
            transform.localPosition = new Vector3(0, 1, 0);
            CurrentState = AgentStateType.Idle;
        }

        resource = null;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // target location
        sensor.AddObservation(Target.Location.x); //1
        sensor.AddObservation(Target.Location.z); //1

        // goal info
        sensor.AddObservation(Goal.transform.localPosition.x); //1
        sensor.AddObservation(Goal.transform.localPosition.z); //1

        // Agent data
        sensor.AddObservation(HasResource); //1
        sensor.AddObservation(transform.localPosition.x); //1
        sensor.AddObservation(transform.localPosition.z); //1
        sensor.AddObservation(Body.velocity.x); //1
        sensor.AddObservation(Body.velocity.z); //1
        sensor.AddObservation((int)CurrentState); // 1
        sensor.AddObservation(IsAtResource); // 1
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        //TODO : refactor ... maybe use events / delegates?
        if (StateDictionary[CurrentState].IsFinished)
        {
            CollectResource();
        }

        if (StateDictionary[CurrentState].IsFinished)
        {
            Move(vectorAction);
        }
    }

    private void CollectResource()
    {
        if (IsAtResource && !HasResource)
        {
            InternalStepCount = 0;
            CurrentState = AgentStateType.Interact;
            StateDictionary[CurrentState].SetAction(TakeResource);
        }
    }

    private void TakeResource()
    {
        if (IsAtResource && !HasResource)
        {
            resource = Target.GetResource();

            if (resource is object)
            {
                InternalStepCount = 0;
                AddReward(0.1f);
                Debug.Log($"Current Reward: {GetCumulativeReward()}");
            }
        }
    }

    /// <summary>
    /// Checks if the goal has the required number of resources from current target.
    /// </summary>
    protected void ValidateJobComplete()
    {
        var isResourceRequired = Goal.GetResourcesRequired().Any(g => g.Key == Target.GetResourceType() && g.Value > 0);

        if (!isResourceRequired)
        {
            onTaskDone?.Invoke(this);
        }
    }

    protected void ValidateGoalComplete()
    {
        if (Goal.IsComplete)
        {
            AddReward(2.0f);
            EndEpisode();
        }
    }

    public override void UpdateTarget(IEnumerable<BaseTarget> baseTargets)
    {
        Target = baseTargets.FirstOrDefault(t => t.IsValid && t is BaseSource) as BaseSource;

        // TODO : add null validation
    }
}
