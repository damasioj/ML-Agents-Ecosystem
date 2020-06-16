﻿using UnityEngine;

/// <summary>
/// This is a basic enemy only for demonstration purposes.
/// </summary>
public class Enemy : MonoBehaviour
{
    /// <summary>
    /// The agent which the enemy should follow.
    /// </summary>
    [SerializeField] private BasicAgent agent;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float acceleration;
    private Rigidbody rBody;
    private Vector3 startingPosition;

    public Vector3 Location
    {
        get
        {
            return transform.position;
        }
        private set
        {
            transform.position = value;
        }
    }

    public Vector3 Velocity => rBody.velocity;

    void Start()
    {
        rBody = GetComponent<Rigidbody>();
        startingPosition = transform.position;
    }

    void Update()
    {
        UpdateLocation();
    }

    public void Reset()
    {
        transform.position = startingPosition;
        rBody.velocity = Vector3.zero;
        rBody.angularVelocity = Vector3.zero;
    }

    void UpdateLocation()
    {
        float x, z;

        if (agent.transform.localPosition.x > Location.x)
        {
            x = acceleration;
        }
        else
        {
            x = -acceleration;
        }

        if (agent.transform.localPosition.z > Location.z)
        {
            z = acceleration;
        }
        else
        {
            z = -acceleration;
        }

        Move(x, z);
    }

    void Move(float x, float z)
    {
        x = x > maxSpeed ? 0 : x;
        z = z > maxSpeed ? 0 : z;

        if (x != 0 && z != 0)
        {
            rBody.AddForce(new Vector3(x, 0, z));
        }
    }
}
