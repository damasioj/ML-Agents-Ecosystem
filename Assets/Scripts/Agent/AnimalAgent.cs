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
    /// <summary>
    /// Amount of energy the animal begins with.
    /// </summary>
    public float initialEnergy;

    private float _energy;
    protected float Energy
    {
        get
        {
            return initialEnergy;
        }
        set
        {
            _energy += value;

            if (_energy > initialEnergy)
            {
                _energy = initialEnergy;
            }
        }
    }
    private bool IsKilled { get; set; }
    private bool HitTarget { get; set; }
    new public FoodSource Target { get; set; } // To reduce conversion costs when getting data from FoodSource

    private bool hitBoundary;
    private Rigidbody rBody;
    private bool raycastHit;
    private Vector3 previousPosition;
    private Animator animator;
    private int layerMask;

    private void Start()
    {
        Energy = initialEnergy;
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
            //if (IsKilled)
            //{
            //    IsDoneCalled = true;
            //    animator.SetInteger("AnimIndex", 2);
            //    animator.SetTrigger("Next");
            //    EndEpisode();
            //}

            // if agent is at target, consume it
            if (HitTarget && Target is object)
            {
                Energy += Target.Consume(1);

                if (Target.IsConsumed)
                {
                    OnTaskDone();
                }
            }
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
        if (!IsDoneCalled)
        {
            // target
            if (Target is object) // if there is an existing food source nearby
            {
                sensor.AddObservation(Target.transform.position); // 3
                sensor.AddObservation(HitTarget); // 1
                sensor.AddObservation(Target.ResourceCount); // 1
            }
            else
            {
                sensor.AddObservation(Vector3.zero); // 3
                sensor.AddObservation(false); // 1
                sensor.AddObservation(0); // 1
            }

            // agent
            sensor.AddObservation(transform.position.x); // 1
            sensor.AddObservation(transform.position.z); // 1
            sensor.AddObservation(rBody.velocity.x); // 1
            sensor.AddObservation(rBody.velocity.z); // 1
            sensor.AddObservation(raycastHit); // 1
            sensor.AddObservation(Energy); // 1

            // enemy
            sensor.AddObservation(enemy.transform.position.x); // 1
            sensor.AddObservation(enemy.transform.position.z); // 1
            sensor.AddObservation(enemy.Velocity); // 3
        }
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        // Animal died
        //if (Energy > 0 && initialEnergy <= 0 && !IsDoneCalled)
        //{
        //    IsDoneCalled = true;
        //    SubtractReward(0.1f);
        //    Debug.Log($"Current Reward: {GetCumulativeReward()}");
        //    EndEpisode();
        //}

        // Move
        Move(vectorAction);

        initialEnergy--;
    }

    protected virtual void VerifyRaycast()
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
        // get the closest target to the animal agent
        var targetsOrdered = baseTargets.OrderBy(t => ObjectHelper.GetDistance(t.gameObject, gameObject));
        Target = targetsOrdered.FirstOrDefault(t => t is FoodSource fs && !fs.IsConsumed) as FoodSource;
    }
}
