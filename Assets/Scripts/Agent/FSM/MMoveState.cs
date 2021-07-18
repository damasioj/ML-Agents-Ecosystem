using System;
using UnityEngine;

/// <summary>
/// To keep similarities with the RL-FSM system, we use a MoveState for the non-RL FSM architecture.
/// Typically, since movement can be easily generalized, this logic is defined on the base subject,
/// and only action-specific states are used.
/// </summary>
public class MMoveState : ManualState
{
    private Vector3 _lastPosition = Vector3.zero;
    public override bool IsFinished { get; protected set; }

    public MMoveState(ManualAgent owner)
        : base(owner) { }

    public override void DoAction()
    {
        return;
    }

    public override void DoAction(float[] input)
    {
        var rBody = Owner.GetComponent<Rigidbody>();
        var scale = Owner.gameObject.transform.localScale.x;

        if (rBody is object)
        {
            Vector3 direction = Vector3.zero;
            direction.x = input[0];
            direction.z = input[1];

            rBody.AddForce(new Vector3(direction.x * Owner.acceleration * scale, 0, direction.z * Owner.acceleration * scale));
        }

        SetDirection();
        _lastPosition = Owner.transform.position;
    }

    public override void OnEnter()
    {
        IsFinished = false;
    }

    public override void OnExit()
    {
        return;
    }

    public override void OnFixedUpdate()
    {
        float[] movement = BasicPathfinder.GetDirection(Owner.Body.transform.localPosition, Owner.GetDestination());

        if (!(movement[0] == 0 && movement[1] == 0))
        {
            IsFinished = false;
            DoAction(movement);
        }
        else
        {
            IsFinished = true;
        }
    }

    public override void OnUpdate()
    {
        return;
    }

    public override void SetAction(Action action, float duration = 0f)
    {
        return;
    }

    private void SetDirection()
    {
        var direction = (Owner.transform.position - _lastPosition).normalized;

        if (Owner.transform.rotation != Quaternion.LookRotation(direction))
        {
            Owner.transform.rotation = Quaternion.Slerp(Owner.transform.rotation, Quaternion.LookRotation(direction), 0.08F);
        }
    }
}
