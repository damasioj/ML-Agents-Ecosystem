using UnityEngine;

public class FoodSource : BaseSource<BambooResource>
{
    /// <summary>
    /// The amount of energy that this food source provides when consumed.
    /// </summary>
    [SerializeField] protected float energyValue;
    /// <summary>
    /// The variable amount of resources this source can have when it spawns.
    /// </summary>
    [SerializeField] protected float resourceCountRange;
    [HideInInspector] public bool IsConsumed => ResourceCount <= 0;

    private void Start()
    {
        SourceHit = false;
        //ResetCollection((int)Random.Range(10f, resourceCountRange));
    }

    public float Consume(int value)
    {
        for (int i = 0; i < value; i++)
        {
            TakeResource();
        }

        return energyValue;
    }
}
