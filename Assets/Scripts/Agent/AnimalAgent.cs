using System;
using System.Collections.Generic;
using System.Linq;
using Unity.MLAgents.Sensors;
using UnityEngine;

/// <summary>
/// Represents an agent with the goal of finding food and avoiding dangers.
/// Has only very basic environment awareness to ensure simplicity.
/// </summary>
public class AnimalAgent : BasicAgent
{
    [SerializeField] Enemy enemy; // just for demonstration purposes    
    /// <summary>
    /// Amount of energy the animal begins with.
    /// </summary>
    public float initialEnergy;

    private float _energy;
    protected float Energy
    {
        get
        {
            return _energy;
        }
        set
        {
            _energy = value;

            if (_energy > initialEnergy)
            {
                _energy = initialEnergy;
            }
        }
    }
    private bool IsKilled { get; set; }
    private bool HitTarget { get; set; }
    private bool StartedConsumption { get; set; }
    new public FoodSource Target { get; set; } // To reduce conversion costs when getting data from FoodSource

    private Rigidbody rBody;
    private Vector3 previousPosition;
    private Animator animator;

    private void Start()
    {
        Energy = initialEnergy;
        HitTarget = false;
        IsKilled = false;
        rBody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    //void FixedUpdate()
    //{
    //    if (!IsDoneCalled)
    //    {
    //        // check if agent died or hit a boundary and reset episode
    //        //if (IsKilled)
    //        //{
    //        //    IsDoneCalled = true;
    //        //    animator.SetInteger("AnimIndex", 2);
    //        //    animator.SetTrigger("Next");
    //        //    EndEpisode();
    //        //}

    //        // if agent is at target, consume it
    //        //if (HitTarget && Target is object)
    //        //{
    //        //    Energy += Target.Consume(1);             

    //        //    if (Target.IsConsumed)
    //        //    {
    //        //        SetReward(1.5f);
    //        //        Debug.Log($"ANIMAL :: Energy = {Energy} // Reward = {GetCumulativeReward()}");
    //        //        OnTaskDone();
    //        //    }
    //        //}
    //    }
    //}

    public override void OnEpisodeBegin()
    {
        base.OnEpisodeBegin();
        HitTarget = false;
        IsKilled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case "food":
                if (other.gameObject.Equals(Target.gameObject))
                {
                    HitTarget = true;
                }
                break;
            case "enemy":
                if (!IsKilled)
                {
                    IsKilled = true;
                    SubtractReward(0.2f);
                    Debug.Log($"ANIMAL :: Current Reward: {GetCumulativeReward()}");
                }
                break;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("food") && other.gameObject.Equals(Target?.gameObject))
        {
            HitTarget = false;
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        if (!IsDoneCalled && Target is object)
        {
            if (Target is object)
            {
                // target
                sensor.AddObservation(Target.Location.x); // 2
                sensor.AddObservation(Target.Location.z);
                sensor.AddObservation(HitTarget); // 1
                sensor.AddObservation(Target.ResourceCount); // 1
            }
            else
            {
                //sensor.AddObservation(Vector3.zero);
                sensor.AddObservation(0f);
                sensor.AddObservation(0f);
                sensor.AddObservation(false);
                sensor.AddObservation(0f);
            }

            // agent
            sensor.AddObservation(transform.localPosition.x); // 1
            sensor.AddObservation(transform.localPosition.z); // 1
            sensor.AddObservation(rBody.velocity.x); // 1
            sensor.AddObservation(rBody.velocity.z); // 1
            sensor.AddObservation(Energy); // 1

            // enemy
            sensor.AddObservation(enemy.transform.localPosition.x); // 1
            sensor.AddObservation(enemy.transform.localPosition.z); // 1
            sensor.AddObservation(enemy.Velocity); // 3
        }
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        // Animal died
        //if (Energy <= 0 && initialEnergy > 0 && !IsDoneCalled)
        //{
        //    IsDoneCalled = true;
        //    SubtractReward(0.1f);
        //    Debug.Log($"ANIMAL :: Current Reward: {GetCumulativeReward()}");
        //    //EndEpisode();
        //}

        // Move
        Move(vectorAction);

        // Action (eat)
        if (Convert.ToBoolean(vectorAction[2]))
        {
            TryConsume();
        }

        Energy--;

        if (Energy <= 100) // for testing only
        {
            Energy += 1000;
        }
    }

    protected virtual void TryConsume()
    {
        // if agent is at target, consume it
        if (HitTarget && Target is object)
        {
            if (!Target.IsConsumed)
            {
                if (!StartedConsumption)
                {
                    AddReward(0.75f);
                    StartedConsumption = true;
                    Debug.Log($"ANIMAL :: Current Reward: {GetCumulativeReward()}");
                }

                float targetEnergy = Target.Consume(1);
                Energy += targetEnergy;

                if (Target.IsConsumed)
                {
                    AddReward(0.75f);
                    HitTarget = false;
                    OnTaskDone();
                    Debug.Log($"ANIMAL :: Current Reward: {GetCumulativeReward()}");
                }
            }
        }
    }

    public override void Heuristic(float[] actions)
    {
        actions[0] = Input.GetAxis("Horizontal");
        actions[1] = Input.GetAxis("Vertical");
        actions[2] = Convert.ToSingle(Input.GetKey(KeyCode.E));
    }

    public override void UpdateTarget(IEnumerable<BaseTarget> baseTargets)
    {
        // get the closest target to the animal agent
        var targetsOrdered = baseTargets.OrderBy(t => ObjectHelper.GetDistance(t.gameObject, gameObject));
        Target = targetsOrdered.FirstOrDefault(t => t is FoodSource fs && !fs.IsConsumed) as FoodSource;

        if (Target == null)
        {
            Debug.Log("ANIMAL :: No targets to consume.");
        }

        HitTarget = false;
    }
}
