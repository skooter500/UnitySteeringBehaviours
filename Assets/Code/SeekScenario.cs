using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class SeekScenario : Scenario
{
        
    public override string Description()
    {
        return "Seek Demo";
    }

    public override void SetUp()
    {

        Params.Load("default.txt");
        leader = (GameObject) GameManager.Instantiate(leader);
        leader.transform.localScale = Vector3.one;

        leader.AddComponent<SteeringBehaviours>();
        leader.transform.position = new Vector3(-10, 20, 20);
        leader.GetComponent<SteeringBehaviours>().turnOn(SteeringBehaviours.behaviour_type.seek);
        leader.GetComponent<SteeringBehaviours>().turnOn(SteeringBehaviours.behaviour_type.obstacle_avoidance);
        leader.GetComponent<SteeringBehaviours>().turnOn(SteeringBehaviours.behaviour_type.wall_avoidance);
        leader.GetComponent<SteeringBehaviours>().seekTargetPos = new Vector3(10, 30, 20);

        //XNAGame.Instance.Leader = leader;

        GameObject camFighter = new GameObject();
        camFighter.AddComponent<SteeringBehaviours>();
        camFighter.GetComponent<SteeringBehaviours>().leader = leader;
        camFighter.GetComponent<SteeringBehaviours>().offset = new Vector3(0, 5, 10);
        camFighter.transform.position = leader.transform.position + camFighter.GetComponent<SteeringBehaviours>().offset;
        camFighter.GetComponent<SteeringBehaviours>().turnOn(SteeringBehaviours.behaviour_type.offset_pursuit);
        camFighter.GetComponent<SteeringBehaviours>().turnOn(SteeringBehaviours.behaviour_type.wall_avoidance);
        camFighter.GetComponent<SteeringBehaviours>().turnOn(SteeringBehaviours.behaviour_type.obstacle_avoidance);
        GameManager.Instance().camFighter = camFighter;
    }

    public override void Update()
    {
        GameManager.PrintMessage("Leader: " + leader);
        GameManager.PrintVector("Seek leader Pos: ", leader.transform.position);
    }

    public override void TearDown()
    {

    }
}