using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class TankAi : Agent
{
    public float Rotate = 2f;
    public float moveForce = 100f;

    public bool trainingMode;
    private Rigidbody rBody;
    private bool Dead;

    public override void Initialize()
    {
        rBody = GetComponent<Rigidbody>();
    }
    public override void OnEpisodeBegin()
    {
       
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(this.transform.localPosition);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        
    }
}
