using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class PlayerAgent : Agent
{

    Rigidbody2D rBody;
    //void Start()
    //{
    //    rBody = GetComponent<Rigidbody2D>();
    //}

    [SerializeField] public Transform Target;
    [SerializeField] public Transform Death;

    CharacterState characterState;

    private void Start()
    {
        rBody = GetComponent<Rigidbody2D>(); 
        characterState = GetComponent<CharacterState>();
    }

    public override void OnEpisodeBegin()
    {
        System.Console.WriteLine("d");
        float[] possiblePositionX = new float[] { -3.5f, 3.5f };
        int randomIndex = Random.Range(0, possiblePositionX.Length);
        float positionX = possiblePositionX[randomIndex];

        Target.transform.position = new Vector3(positionX, 0, 0);
        Death.transform.position = (randomIndex == 0) ? new Vector3(possiblePositionX[1], 0, 0) : new Vector3(possiblePositionX[0], 0, 0);

        transform.position = new Vector3(0, 0, 0);
        //if (this.transform.localPosition.y < 0)
        //{
        //    //this.rBody.angularVelocity = Vector2.zero;
        //    this.rBody.velocity = Vector2.zero;
        //    this.transform.localPosition = new Vector2(0, 0);
        //}



        //Target.localPosition = new Vector2(Random.value * 8 - 4,

        //                                   Random.value * 8 - 4);
    }

    public float forceMultiplier = 10;
    public override void OnActionReceived(ActionBuffers actions)
    {
        characterState.horizontal = actions.ContinuousActions[0];
        Vector2 controlSignal = Vector2.zero;
        controlSignal.x = actions.ContinuousActions[0];
        //controlSignal.y = actionBuffers.ContinuousActions[1];
        rBody.AddForce(controlSignal * forceMultiplier);

        float distanceToTarget = Vector2.Distance(this.transform.localPosition, Target.localPosition);
        // reached target
        //if (distanceToTarget < .42f)
        //{
        //   SetReward(1.0f);
        //   EndEpisode();
        //}
        // died
        //else if (this.transform.localPosition.y < 0)
        //{
        //    EndEpisode();
        //}
    }

    //public override void Heuristic(in ActionBuffers actionsOut)
    //{
    //    var continuousActionsOut = actionsOut.ContinuousActions;
    //    continuousActionsOut[0] = Input.GetAxis("Horizontal");
    //    continuousActionsOut[1] = Input.GetAxis("Vertical");
    //}

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name.Equals("Target"))
        {
            Debug.Log("Target reached!");
            SetReward(+20f);
            EndEpisode();
        }
        else if (collision.gameObject.name.Equals("Crystal_01"))
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
        

        // Agent velocity
        //sensor.AddObservation(rBody.velocity.x);
        //sensor.AddObservation(rBody.velocity.y);
    }



    void fixedUpdate()
    {
        
    }



}
