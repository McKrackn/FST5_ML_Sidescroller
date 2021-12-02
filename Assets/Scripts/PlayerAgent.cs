using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class PlayerAgent : Agent
{

    Rigidbody2D rBody;

    [SerializeField] public Transform Target;
    [SerializeField] public Transform Death;

    CharacterState characterState;
    private float distance2Target;

    private void Start()
    {
        rBody = GetComponent<Rigidbody2D>(); 
        characterState = GetComponent<CharacterState>();
    }

    public override void OnEpisodeBegin()
    {
        float[] possiblePositionX = new float[] { -3.5f, 3.5f };
        int randomIndex = Random.Range(0, possiblePositionX.Length);
        float positionX = possiblePositionX[randomIndex];

        Target.transform.position = new Vector3(positionX, 0, 0);
        Death.transform.position = (randomIndex == 0) ? new Vector3(possiblePositionX[1], 0, 0) : new Vector3(possiblePositionX[0], 0, 0);

        transform.position = new Vector3(0, 0, 0);
        distance2Target= Vector2.Distance(this.transform.localPosition, Target.localPosition);
    }

    public float forceMultiplier = 10;
    public override void OnActionReceived(ActionBuffers actions)
    {
        characterState.horizontal = actions.ContinuousActions[0];
        Vector2 controlSignal = Vector2.zero;

        controlSignal.x = actions.ContinuousActions[0];
        controlSignal.y = actions.ContinuousActions[1];

        rBody.AddForce(controlSignal * forceMultiplier);

        float newDistance2Target = Vector2.Distance(this.transform.localPosition, Target.localPosition);
        if (newDistance2Target < distance2Target) { AddReward(0.5f); }
        else { AddReward(-0.5f); }

        distance2Target = newDistance2Target;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name.Equals("Key"))
        {
            Debug.Log("Target reached!");
            SetReward(+20f);
            EndEpisode();
        }
        else if (collision.gameObject.name.Equals("Spikes"))
        {
            Debug.Log("Agents Heaven reached!");
            SetReward(-20f);
            EndEpisode();
        }
    }

    // How far KI can see
    public override void CollectObservations(VectorSensor sensor) 
    {
        // Target and Agent positions
        sensor.AddObservation(transform.position); 
        sensor.AddObservation(Target.localPosition);
        sensor.AddObservation(Death.localPosition);
        
    }



    void fixedUpdate()
    {
        
    }



}
