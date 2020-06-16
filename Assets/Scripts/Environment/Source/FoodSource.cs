using UnityEngine;

public class FoodSource : BaseSource<BambooResource>
{
    private void Awake()
    {
        SourceHit = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        SourceHit = true;
    }
}
