using System;
using System.Collections.Generic;
using System.Linq;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class HaulerAgent : BasicAgent
{
    // agent
    GameObject agentHead;
    bool targetRaycast;

    // target
    float lastTargetDistance;
    Rigidbody targetBody;
    Vector3 targetDimensions;
    IMovable movableTarget;

    // env
    List<bool> raycastsHit;
    List<GameObject> obstacles;

    Vector3[] directions = new Vector3[3] // add to raycast helper
    {
        Vector3.forward,
        new Vector3(0.5f, 0, 0.5f),
        new Vector3(-0.5f, 0, 0.5f)
    };

    public override BaseTarget Target 
    { 
        get => base.Target; 
        set
        {
            base.Target = value;
            movableTarget = value as IMovable;
        }
    }

    public HaulerAgent() : base()
    {
        if (!(Target is IMovable))
        {
            throw new System.Exception("Target provided to hauler agent is not IMovable!");
        }

        lastTargetDistance = 0f;

        targetBody = Target.GetComponent<Rigidbody>();
        agentHead = GetComponentInChildren<SphereCollider>().gameObject;

        raycastsHit = new List<bool>() { false, false, false }; // refactor this
        obstacles = new List<GameObject>() { null, null, null };
    }

    //private IList<T> InitializeListWithNull<T>(int amount)
    //{
    //    List<T> temp = new List<T>();

    //    for (int i = 0; i < amount; i++)
    //    {
    //        temp.Add((T)null);
    //    }

    //    return temp;
    //}

    void Update()
    {
        float distance = ObjectHelper.EvaluateProximity(ref lastTargetDistance, Target.gameObject, Goal.gameObject);

        if (distance > 0)
        {
            InternalStepCount = StepCount;
            AddReward(distance * 0.0001f);
        }

        if (StepCount - InternalStepCount > maxInternalSteps && !IsDoneCalled)
        {
            IsDoneCalled = true;
            SubtractReward(0.1f);
            Debug.Log($"Reward: {GetCumulativeReward()}");
            Debug.Log($"No point earned in last {maxInternalSteps} steps. Restarting ...");
            EndEpisode();
        }
    }

    void FixedUpdate()
    {
        ExecuteRaycasts();
    }

    public override void OnEpisodeBegin()
    {
        SetReward(0f);

        Body.angularVelocity = Vector3.zero;
        Body.velocity = Vector3.zero;

        targetDimensions = ObjectHelper.GetDimensions(Target.gameObject);
        lastTargetDistance = 0f;
        InternalStepCount = StepCount;
        IsDoneCalled = false;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // target location
        sensor.AddObservation(Target.transform.position); //3
        sensor.AddObservation(targetBody.velocity); //3
        sensor.AddObservation(targetDimensions); //3
        sensor.AddObservation(Target.transform.rotation); //3
        sensor.AddObservation((int)movableTarget.Shape);

        // goal info
        sensor.AddObservation(Goal.transform.position); //3

        // Agent data
        sensor.AddObservation(transform.position); //3
        sensor.AddObservation(Body.velocity); //3
        sensor.AddObservation(targetRaycast);

        // obstacle info
        raycastsHit.ForEach(x => sensor.AddObservation(x)); // n * 1

        for (int i = 0; i < obstacles.Count; i++)
        {
            sensor.AddObservation(ObjectHelper.GetDimensions(obstacles[i])); // n * 3

            if (obstacles[i] == null)
            {
                sensor.AddObservation(Vector3.zero);
                continue;
            }

            sensor.AddObservation(obstacles[i].transform.position); // n * 3
        }
    }

    public override void UpdateTarget(IEnumerable<BaseTarget> baseTargets)
    {
        Target = baseTargets.FirstOrDefault(t => t is IMovable);

        // TODO : add null validation
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        Move(vectorAction);
    }

    // TODO : check if this is needed
    protected override void Move(float[] vectorAction)
    {
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = vectorAction[0];
        controlSignal.z = vectorAction[1];

        // agent is idle
        if (controlSignal.x == 0 && controlSignal.z == 0)
        {
            Body.angularVelocity = Vector3.zero;
            Body.velocity = Vector3.zero;
            return;
        }

        // agent is moving
        if (Body.velocity.x > 30)
        {
            controlSignal.x = 0;
        }
        if (Body.velocity.z > 30)
        {
            controlSignal.z = 0;
        }

        Body.AddForce(new Vector3(controlSignal.x * 750, 0, controlSignal.z * 750));

        SetDirection();
    }

    /// <summary>
    /// Used by exetrnal sources to mark that the agent finished the task.
    /// </summary>
    public void MarkTaskDone()
    {
        if (!IsDoneCalled)
        {
            IsDoneCalled = true;
            SetReward(3f);
            //EndEpisode();
            onTaskDone?.Invoke(this);
        }
    }

    private void ExecuteRaycasts() // TODO :refactor
    {
        // Bit shift the index of the layer (8) to get a bit mask
        int layerMask = 1 << 8;

        // This would cast rays only against colliders in layer 8.
        // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
        layerMask = ~layerMask;

        for (int i = 0; i < directions.Length; i++)
        {
            if (Physics.Raycast(agentHead.transform.position, agentHead.transform.TransformDirection(directions[i]), out RaycastHit hit, 20f, layerMask))
            {
                Debug.DrawRay(agentHead.transform.position, agentHead.transform.TransformDirection(directions[i]) * hit.distance, Color.red);

                ValidateRaycastCollision(hit, i);
            }
            else
            {
                Debug.DrawRay(agentHead.transform.position, agentHead.transform.TransformDirection(directions[i]) * 20f, Color.white);

                raycastsHit[i] = false;
                obstacles[i] = null;
            }
        }

        // check target raycast ... we want this to be separate from others
        Vector3 downDirection = new Vector3(0, -0.3f, 1f);
        if (Physics.Raycast(agentHead.transform.position, agentHead.transform.TransformDirection(downDirection), out RaycastHit targHit, 7f, layerMask))
        {
            Debug.DrawRay(agentHead.transform.position, agentHead.transform.TransformDirection(downDirection) * targHit.distance, Color.red);
            ValidateRaycastCollision(targHit, 0);
        }
        else
        {
            Debug.DrawRay(agentHead.transform.position, agentHead.transform.TransformDirection(downDirection) * 7f, Color.white);
            targetRaycast = false;
        }

    }

    private void ValidateRaycastCollision(RaycastHit hit, int index)
    {
        if (hit.collider.CompareTag("obstacle"))
        {
            raycastsHit[index] = true;
            obstacles[index] = hit.collider.gameObject;
        }
        else if (hit.collider.CompareTag("target"))
        {
            targetRaycast = true;
        }
        else
        {
            raycastsHit[index] = false;
            obstacles[index] = null;
        }
    }
}
