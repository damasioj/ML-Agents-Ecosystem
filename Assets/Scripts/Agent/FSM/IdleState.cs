using System;
using UnityEngine;

public class IdleState : AgentState
{
    public override bool IsFinished { get; protected set; } = true;

    public override void SetAction(Action action)
    {
        return;
    }

    public override void DoAction(BasicAgent owner)
    {
        return;
    }

    public override void DoAction(BasicAgent owner, float[] vectorActions)
    {
        return;
    }

    public override void OnEnter(BasicAgent owner)
    {
        IsFinished = false;
        var rBody = owner.GetComponent<Rigidbody>();
        if (rBody is object)
        {
            rBody.angularVelocity = Vector3.zero;
            rBody.velocity = Vector3.zero;
        }
        IsFinished = true;
    }

    public override void OnExit(BasicAgent owner)
    {
        return;
    }

    public override void OnFixedUpdate(BasicAgent owner)
    {
        return;
    }

    public override void OnUpdate(BasicAgent owner)
    {
        return;
    }
}
