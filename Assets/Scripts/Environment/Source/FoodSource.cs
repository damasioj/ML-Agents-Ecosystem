using UnityEngine;

public class FoodSource : BaseSource<BambooResource>
{
    [HideInInspector] public bool IsConsumed => hp <= 0;
    public int hp;

    private void Awake()
    {
        SourceHit = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        SourceHit = true;
    }
}
