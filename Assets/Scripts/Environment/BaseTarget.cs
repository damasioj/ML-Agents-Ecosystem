using UnityEngine;

public abstract class BaseTarget : MonoBehaviour
{
    public virtual bool IsValid { get; set; }

    public virtual Vector3 Location
    {
        get
        {
            return gameObject.transform.position;
        }
        protected set
        {
            gameObject.transform.position = value;
        }
    }
}
