using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Represents a collector agent that uses FSM only.
/// </summary>
public class CollectorAgentFSM : ManualAgent, IHasGoal
{
    private BaseResource resource;
    private bool HasResource => resource is object;
    private bool IsAtSource { get; set; }
    private bool IsAtGoal { get; set; }
    new public BaseSource Target { get; set; }
    public BaseStructure Goal { get; private set; }

    protected override void OnStart()
    {
        AssignStateDictionary();
    }

    protected override void OnUpdate()
    {
        // ..
    }

    protected override void OnFixedUpdate()
    {
        DoAction();
    }

    protected override void AssignStateDictionary()
    {
        StateDictionary = new Dictionary<AgentStateType, ManualState>()
        {
            [AgentStateType.Idle] = new MIdleState(this),
            [AgentStateType.Move] = new MMoveState(this),
            [AgentStateType.Interact] = new MInteractState(this)
        };
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case "goal":
                if (HasResource && other.gameObject == Goal.gameObject)
                {
                    var deposit = other.gameObject.GetComponent(typeof(BaseStructure)) as BaseStructure;
                    deposit.AddResource(ref resource);
                    ValidateJobComplete();
                    ValidateGoalComplete();
                    InternalStepCount = 0;
                }
                break;
            case "target":
                if (!HasResource && other.GetComponent<BaseSource>().Equals(Target))
                {
                    IsAtSource = true;
                }
                break;
            case "boundary":
                if (!IsDoneCalled)
                {
                    IsDoneCalled = true;
                }
                break;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("target"))
        {
            IsAtSource = false;
        }
    }

    //public override void CollectObservations(VectorSensor sensor)
    //{
    //    if (Target is object)
    //    {
    //        // target location
    //        sensor.AddObservation(Target.Location.x); //1
    //        sensor.AddObservation(Target.Location.z); //1

    //        // goal info
    //        sensor.AddObservation(Goal.Location.x); //1
    //        sensor.AddObservation(Goal.Location.z); //1

    //        // Agent data
    //        sensor.AddObservation(HasResource); //1
    //        sensor.AddObservation(transform.localPosition.x); //1
    //        sensor.AddObservation(transform.localPosition.z); //1
    //        sensor.AddObservation(Body.velocity.x); //1
    //        sensor.AddObservation(Body.velocity.z); //1
    //        sensor.AddObservation((int)CurrentState); // 1
    //        sensor.AddObservation(IsAtResource); // 1
    //    }
    //}

    //public override void OnActionReceived(float[] vectorAction)
    //{
    //    //TODO : refactor ... maybe use events / delegates?
    //    if (StateDictionary[CurrentState].IsFinished)
    //    {
    //        CollectResource();
    //    }

    //    if (StateDictionary[CurrentState].IsFinished)
    //    {
    //        Move(vectorAction);
    //    }
    //}

    protected void DoAction()
    {
        if (StateDictionary[CurrentState].IsFinished)
        {
            if (HasResource && !IsAtGoal)
            {
                CurrentState = AgentStateType.Move;
            }
            else if (!IsAtSource && !HasResource) // not at target, doesnt have resource
            {
                // if no targets available, make idle and keep checking job status
                if (Target.ResourceCount == 0)
                {
                    CurrentState = AgentStateType.Idle;
                    ValidateJobComplete();
                }
                else // otherwise set move to target
                {
                    CurrentState = AgentStateType.Move;
                }
            }
            else if (IsAtSource && !HasResource) // at target, collect resource
            {
                CollectResource();
            }
            else if (HasResource && IsAtGoal) // has resource, at goal
            {
                // drop resources
                Goal.AddResource(ref resource);
                ValidateJobComplete();
                ValidateGoalComplete();
                InternalStepCount = 0;
            }            
        }
        else // continue current action
        {
            StateDictionary[CurrentState].OnFixedUpdate();
        }
    }

    private void CollectResource()
    {
        InternalStepCount = 0;
        CurrentState = AgentStateType.Interact;
        StateDictionary[CurrentState].SetAction(TakeResource, 50f);
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
            Debug.Log("COLLECTOR :: Job complete.");
        }
    }

    public override Vector3 GetDestination()
    {
        if (HasResource)
        {
            return Goal.Location;
        }
        else
        {
            return Target.Location;
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
