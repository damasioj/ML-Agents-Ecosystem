using System;
using Unity.MLAgents;
using UnityEngine;

public class IdleState : AgentState
{
    public override bool IsFinished { get; protected set; } = true;

    public override void SetAction(Action action)
    {
        return;
    }

    public override void DoAction(Agent owner)
    {
        return;
    }

    public override void DoAction(Agent owner, float[] vectorActions)
    {
        return;
    }

    public override void OnEnter(Agent owner)
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

    public override void OnExit(Agent owner)
    {
        return;
    }

    public override void OnFixedUpdate(Agent owner)
    {
        return;
    }

    public override void OnUpdate(Agent owner)
    {
        return;
    }
}
