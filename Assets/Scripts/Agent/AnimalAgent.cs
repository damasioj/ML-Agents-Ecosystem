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
    [SerializeField] Enemy enemy; // TODO : divert to environment manager
    public float energy;    
    private float InitialEnergy { get; set; }
    private bool IsKilled { get; set; }
    private bool HitTarget { get; set; }

    // target data
    public List<BaseTarget> targets;

    private bool hitBoundary;
    private Rigidbody rBody;
    private bool raycastHit;
    private Vector3 previousPosition;
    private Animator animator;
    private int layerMask;

    private void Start()
    {
        InitialEnergy = energy;
        HitTarget = false;
        IsKilled = false;
        raycastHit = false;
        rBody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        hitBoundary = false;
        layerMask = 0 << 8;
        layerMask = ~layerMask;
    }

    private void Update()
    {
        
    }

    void FixedUpdate()
    {
        if (!IsDoneCalled)
        {
            // check if agent died or hit a boundary and reset episode
            if (IsKilled)
            {
                IsDoneCalled = true;
                animator.SetInteger("AnimIndex", 2);
                animator.SetTrigger("Next");
                EndEpisode();
            }

            // if agent is at target, consume it
            //if (activeTarget is IConsumable cons)
            //{
            //    if (!cons.IsConsumed)
            //    {
            //        bool isConsumed = cons.Consume(1f);

            //        energy += 10;
            //        AddReward(0.01f);
            //        Debug.Log($"Current Reward: {GetCumulativeReward()}");

            //        if (energy > InitialEnergy)
            //        {
            //            energy = InitialEnergy;
            //        }

            //        if (isConsumed && targets.Cast<FoodSource>().All(t => t.IsConsumed))
            //        {
            //            isDoneCalled = true;
            //            EndEpisode();
            //        }
            //    }
            //}
        }

        VerifyRaycast();
    }

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
                HitTarget = true;
                Target = other.gameObject.GetComponent<BaseTarget>();
                break;
            case "enemy":
                if (!IsKilled)
                {
                    IsKilled = true;
                    SubtractReward(0.2f);
                    Debug.Log($"Current Reward: {GetCumulativeReward()}");
                }
                break;
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // target
        targets.ForEach(t => sensor.AddObservation(t.transform.position)); // n * 3
        targets.Cast<FoodSource>().ToList().ForEach(t => sensor.AddObservation(t.IsConsumed)); // n * 1
        targets.Cast<FoodSource>().ToList().ForEach(t => sensor.AddObservation(t.hp)); // n * 1
        sensor.AddObservation(Target is object); // 1

        // agent
        sensor.AddObservation(transform.position); // 3
        sensor.AddObservation(rBody.velocity.x); // 1
        sensor.AddObservation(rBody.velocity.z); // 1
        sensor.AddObservation(raycastHit); // 1
        sensor.AddObservation(energy); // 1

        // enemy
        sensor.AddObservation(enemy.transform.position); // 3
        sensor.AddObservation(enemy.Velocity); // 3
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        // Animal died
        if (InitialEnergy > 0 && energy <= 0 && !IsDoneCalled)
        {
            IsDoneCalled = true;
            SubtractReward(0.1f);
            Debug.Log($"Current Reward: {GetCumulativeReward()}");
            EndEpisode();
        }

        // Move
        Move(vectorAction);

        energy--;
    }

    private void VerifyRaycast()
    {
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out RaycastHit hit, 50f, layerMask))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.red);

            if (hit.collider.CompareTag("obstacle"))
            {
                raycastHit = true;
            }
        }
        else
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 50f, Color.white);

            raycastHit = false;
        }
    }

    public override void UpdateTarget(IEnumerable<BaseTarget> baseTargets)
    {
        targets.Clear();
        targets.AddRange(baseTargets);
        //Target = baseTargets.FirstOrDefault(t => t.IsValid && t is BaseSource) as BaseSource;

        // TODO : add null validation
    }
}
