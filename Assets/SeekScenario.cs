using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class SeekScenario : Scenario
{
    GameObject leader;

    public override string Description()
    {
        return "Seek Demo";
    }

    public override void SetUp()
    {
        Params.Load("default.properties");

        leader = (GameObject) GameManager.Instantiate(Resources.Load("cobramk3"));
        leader.AddComponent<SteeringBehaviours>();
        leader.transform.position = new Vector3(-10, 20, 20);
        leader.GetComponent<SteeringBehaviours>().turnOn(SteeringBehaviours.behaviour_type.seek);
        leader.GetComponent<SteeringBehaviours>().turnOn(SteeringBehaviours.behaviour_type.obstacle_avoidance);
        leader.GetComponent<SteeringBehaviours>().turnOn(SteeringBehaviours.behaviour_type.wall_avoidance);
        leader.GetComponent<SteeringBehaviours>().seekTargetPos = new Vector3(10, 30, 20);

        //XNAGame.Instance.Leader = leader;

        //Fighter camFighter = new Fighter();
        //camFighter.Leader = leader;
        //camFighter.offset = new Vector3(0, 5, 10);
        //camFighter.Position = leader.Position + camFighter.offset;
        //camFighter.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.offset_pursuit);
        //camFighter.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.wall_avoidance);
        //camFighter.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.obstacle_avoidance);
        //XNAGame.Instance.CamFighter = camFighter;
        //children.Add(camFighter);

        //Ground ground = new Ground();
        //children.Add(ground);

        //XNAGame.Instance.Ground = ground;
        //foreach (Entity child in children)
        //{
        //    child.LoadContent();
        //}
    }

    public override void Update()
    {
        GameManager.PrintVector("Seek leader Pos: ", leader.transform.position);
    }

    public override void TearDown()
    {

    }
}