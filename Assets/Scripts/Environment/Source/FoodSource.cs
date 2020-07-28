using UnityEngine;

public class FoodSource : BaseSource<BambooResource>
{
    /// <summary>
    /// The amount of energy that this food source provides when consumed.
    /// </summary>
    [SerializeField] protected float energyValue;
    [HideInInspector] public bool IsConsumed => ResourceCount <= 0;

    private void Awake()
    {
        SourceHit = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        SourceHit = true;
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
