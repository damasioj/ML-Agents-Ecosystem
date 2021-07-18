using System;
using UnityEngine;

public class MIdleState : ManualState
{
    public override bool IsFinished { get; protected set; } = true;

    public MIdleState(ManualAgent owner)
        : base(owner) { }

    public override void SetAction(Action action, float duration = 0f)
    {
        return;
    }

    public override void DoAction()
    {
        return;
    }

    public override void DoAction(float[] vectorActions)
    {
        return;
    }

    public override void OnEnter()
    {
        IsFinished = false;
        var rBody = Owner.GetComponent<Rigidbody>();
        if (rBody is object)
        {
            rBody.angularVelocity = Vector3.zero;
            rBody.velocity = Vector3.zero;
        }
        IsFinished = true;
    }

    public override void OnExit()
    {
        return;
    }

    public override void OnFixedUpdate()
    {
        return;
    }

    public override void OnUpdate()
    {
        return;
    }
}
