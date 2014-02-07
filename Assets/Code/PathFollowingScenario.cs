using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class PathFollowingScenario:Scenario
{
    public override string Description()
    {
        return "Path Following Demo";
    }
    static Vector3 initialPos = Vector3.zero;

    public override void SetUp()
    {
        Params.Load("default.txt");

        leader = (GameObject)GameManager.Instantiate(leader);
        leader.AddComponent<SteeringBehaviours>();
        leader.transform.position = new Vector3(-20, 5, 50);
       
        if (initialPos == Vector3.zero)
        {
            initialPos = leader.transform.position;
        }
        Path path = leader.GetComponent<SteeringBehaviours>().path;
        leader.GetComponent<SteeringBehaviours>().path.Waypoints.Add(initialPos);
        path.Waypoints.Add(initialPos + new Vector3(-50, 0, -80));
        path.Waypoints.Add(initialPos + new Vector3(0, 0, -160));
        path.Waypoints.Add(initialPos + new Vector3(50, 0, -80));
        path.Looped = true;
        path.draw = true;
        leader.GetComponent<SteeringBehaviours>().turnOffAll();
        leader.GetComponent<SteeringBehaviours>().turnOn(SteeringBehaviours.behaviour_type.follow_path);
        leader.GetComponent<SteeringBehaviours>().turnOn(SteeringBehaviours.behaviour_type.obstacle_avoidance);


        GameObject camFighter = new GameObject();
        camFighter.AddComponent<SteeringBehaviours>();
        camFighter.GetComponent<SteeringBehaviours>().leader = leader;
        camFighter.GetComponent<SteeringBehaviours>().offset = new Vector3(0, 5, 10);
        camFighter.transform.position = leader.transform.position + camFighter.GetComponent<SteeringBehaviours>().offset;
        camFighter.GetComponent<SteeringBehaviours>().turnOn(SteeringBehaviours.behaviour_type.offset_pursuit);
        //camFighter.GetComponent<SteeringBehaviours>().turnOn(SteeringBehaviours.behaviour_type.wall_avoidance);
        //camFighter.GetComponent<SteeringBehaviours>().turnOn(SteeringBehaviours.behaviour_type.obstacle_avoidance);
        GameManager.Instance().camFighter = camFighter;

        
    }

    public override void TearDown()
    {
        
    }
}
