using System.Collections.Generic;
using System.Linq;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class AnimalAgent : BasicAgent
{
    [SerializeField] Enemy enemy; // TODO : divert to environment manager
    public float energy;

    
    private float InitialEnergy { get; set; }
    private bool IsKilled { get; set; }
    private bool HitTarget { get; set; }

    public AnimalAgent() : base()
    {        
        HitTarget = false;
        IsKilled = false;
    }

    private void Start()
    {
        InitialEnergy = energy;
    }

    void FixedUpdate()
    {
        // check if agent died ...
    }

    public override void Reset()
    {
        base.Reset();
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
        // Target
        sensor.AddObservation(Target.transform.localPosition); // 3

        //sensor.AddObservation(cons.IsConsumed);
        sensor.AddObservation(false); // TODO : TEMPORARY 

        // agent
        sensor.AddObservation(transform.localPosition); // 3
        sensor.AddObservation(Body.velocity.x); // 1
        sensor.AddObservation(Body.velocity.z); // 1

        // Energy
        sensor.AddObservation(energy);

        // enemy
        sensor.AddObservation(enemy.Location); // 3
        sensor.AddObservation(enemy.Velocity); // 3
    }

    public override void UpdateTarget(IEnumerable<BaseTarget> baseTargets)
    {
        Target = baseTargets.FirstOrDefault(t => t.IsValid && t is BaseSource) as BaseSource;

        // TODO : add null validation
    }
}
