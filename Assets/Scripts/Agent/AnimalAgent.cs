using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AnimalAgent : BasicAgent
{
    [SerializeField] Enemy enemy;
    public float energy;

    
    private float InitialEnergy { get; set; }
    private bool IsKilled { get; set; }
    private bool HitTarget { get; set; }

    public AnimalAgent() : base()
    {
        InitialEnergy = energy;
        HitTarget = false;
        IsKilled = false;
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

    public override void UpdateTarget(IEnumerable<BaseTarget> baseTargets)
    {
        Target = baseTargets.FirstOrDefault(t => t.IsValid && t is BaseSource) as BaseSource;

        // TODO : add null validation
    }
}
