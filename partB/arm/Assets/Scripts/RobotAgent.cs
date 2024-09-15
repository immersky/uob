using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using System;

public class RobotAgent : Agent
{
    public GameObject endEffector;
    public GameObject cube;
    public GameObject robot;
    private Vector3 previousEndEffectorPosition;
    private float episodeTime;
    private const float maxEpisodeDuration = 10f; // 10 seconds

    RobotController robotController;



    void Start()
    {
        robotController = robot.GetComponent<RobotController>();
  
        episodeTime = 0f;
    }


    // AGENT

    public override void OnEpisodeBegin()
    {
        float[] defaultRotations = { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f };
        robotController.ForceJointsToRotations(defaultRotations);
         episodeTime = 0f; // Reset time

    }
    public override void OnActionReceived(float[] vectorAction)
    {
        episodeTime += Time.deltaTime;
        // Move joints based on actions
        for (int jointIndex = 0; jointIndex < vectorAction.Length; jointIndex++)
        {
            RotationDirection rotationDirection = ActionIndexToRotationDirection((int)vectorAction[jointIndex]);
            robotController.RotateJoint(jointIndex, rotationDirection, false);
        }


        if (Vector3.Dot(endEffector.transform.up, Vector3.up) > 0.99f)
        {
            SetReward(1.0f);
        }
        else
        {
            SetReward(-100*(1.0f-Vector3.Dot(endEffector.transform.up, Vector3.up)));
        }


        if (endEffector.transform.position.x > previousEndEffectorPosition.x)
        {
            SetReward(0.1f);
        }
        previousEndEffectorPosition = endEffector.transform.position;


        float zDistance = Mathf.Abs(endEffector.transform.position.z + 0.5f);  //z----> -0.5
        if (zDistance < 0.02f)
        {
            SetReward(0.1f*10);
        }
        else
        {
            SetReward(zDistance);
        }


        if (Mathf.Abs(endEffector.transform.position.y) < 0.02f)
        {
            SetReward(0.1f*10);
        }
        else
        {
            SetReward(-endEffector.transform.position.y);
        }

        // End episode based on time
        if (episodeTime >= maxEpisodeDuration)
        {
            EndEpisode();
        }
    }



        // HELPERS

    static public RotationDirection ActionIndexToRotationDirection(int actionIndex)
    {
        return (RotationDirection)(actionIndex - 1);
    }




}


