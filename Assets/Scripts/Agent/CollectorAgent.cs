using System.Collections.Generic;
using System.Linq;
using Unity.MLAgents.Sensors;
using UnityEngine;

/// <summary>
/// Represents an agent with the goal of collecting resources and bringing it to a defined goal.
/// </summary>
public class CollectorAgent : BasicAgent, IHasGoal
{    
    private BaseResource resource;
    private bool HasResource => resource is object;
    private bool IsAtResource { get; set; }
    new public BaseSource Target { get; set; }
    public BaseStructure Goal { get; private set; }

    void Start()
    {
        AssignStateDictionary();
    }

    protected override void AssignStateDictionary()
    {
        StateDictionary = new Dictionary<AgentStateType, AgentState>()
        {
            [AgentStateType.Idle] = new IdleState(this),
            [AgentStateType.Move] = new MoveState(this),
            [AgentStateType.Interact] = new InteractState(this)
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
                    Debug.Log($"COLLECTOR :: Current Reward = {GetCumulativeReward()}");
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
                    Debug.Log($"COLLECTOR :: Current Reward = {GetCumulativeReward()}");
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
            CurrentState = AgentStateType.Idle;
        }

        resource = null;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        if (Target is object)
        {
            // target location
            sensor.AddObservation(Target.Location.x); //1
            sensor.AddObservation(Target.Location.z); //1

            // goal info
            sensor.AddObservation(Goal.Location.x); //1
            sensor.AddObservation(Goal.Location.z); //1

            // Agent data
            sensor.AddObservation(HasResource); //1
            sensor.AddObservation(transform.localPosition.x); //1
            sensor.AddObservation(transform.localPosition.z); //1
            sensor.AddObservation(Body.velocity.x); //1
            sensor.AddObservation(Body.velocity.z); //1
            sensor.AddObservation((int)CurrentState); // 1
            sensor.AddObservation(IsAtResource); // 1
        }
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
        if (!HasResource)
        {
            resource = Target.TakeResource();

            if (resource is object)
            {
                InternalStepCount = 0;
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
            OnTaskDone();
        }
    }

    protected void ValidateGoalComplete()
    {
        if (Goal.IsComplete)
        {
            AddReward(2.0f);
            //EndEpisode();
        }
    }

    public override void UpdateTarget(IEnumerable<BaseTarget> baseTargets)
    {
        // updates with a valid target that contains a resource required by the goal
        var resourceTypes = Goal.GetResourcesRequired().Where(g => g.Value > 0).Select(g => g.Key);
        Target = baseTargets.FirstOrDefault(t => t.IsValid 
                                            && t is BaseSource source 
                                            && resourceTypes.Contains(source.GetResourceType())) as BaseSource;

        if (Target == null)
        {
            Debug.Log("COLLECTOR :: No targets.");
        }
    }

    public void UpdateGoal(IEnumerable<BaseStructure> baseStructures)
    {
        if (baseStructures.Count() > 0)
        {
            Goal = baseStructures.First();
        }
        else
        {
            Debug.Log("COLLECTOR :: No new goals.");
        }
    }
}
