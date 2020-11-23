using System;
using UnityEngine;

public class MoveState : AgentState
{
    private Vector3 _lastPosition = Vector3.zero;
    public override bool IsFinished { get; protected set; }

    public override void DoAction(BasicAgent owner)
    {
        return;
    }

    public override void DoAction(BasicAgent owner, float[] vectorAction)
    {
        var rBody = owner.GetComponent<Rigidbody>();
        var scale = owner.gameObject.transform.localScale.x;

        //rBody.angularVelocity = Vector3.zero;
        //rBody.velocity = Vector3.zero;
        //if (animator.GetInteger("AnimIndex") != 0)
        //{
        //    animator.SetInteger("AnimIndex", 0);
        //    animator.SetTrigger("Next");
        //}

        if (rBody is object)
        {
            Vector3 controlSignal = Vector3.zero;
            controlSignal.x = vectorAction[0];
            controlSignal.z = vectorAction[1];

            rBody.AddForce(new Vector3(controlSignal.x * owner.acceleration * scale, 0, controlSignal.z * owner.acceleration * scale));
        }

        SetDirection(owner);
        _lastPosition = owner.transform.position;

        IsFinished = true;
    }

    public override void OnEnter(BasicAgent owner)
    {
        IsFinished = false;
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

    public override void SetAction(Action action)
    {
        return;
    }

    private void SetDirection(BasicAgent owner)
    {
        var direction = (owner.transform.position - _lastPosition).normalized;

        if (owner.transform.rotation != Quaternion.LookRotation(direction))
        {
            owner.transform.rotation = Quaternion.Slerp(owner.transform.rotation, Quaternion.LookRotation(direction), 0.08F);
        }
    }
}
