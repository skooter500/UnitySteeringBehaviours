using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class PursueScenario:Scenario
{
    public override string Description()
    {
        return "Pursuit Demo";
    }
    public override void SetUp()
    {
        Params.Load("default.txt");
        leader = (GameObject) GameObject.Instantiate(leader);
        leader.transform.position = new Vector3(10, 5, 10);
        leader.AddComponent<SteeringBehaviours>();
        leader.GetComponent<SteeringBehaviours>().turnOn(SteeringBehaviours.behaviour_type.seek);
        leader.GetComponent<SteeringBehaviours>().seekTargetPos = new Vector3(100, 5, 100);

        boid = (GameObject)GameObject.Instantiate(boid);
        boid.transform.position = new Vector3(25, 5, 50);
        boid.AddComponent<SteeringBehaviours>();
        boid.GetComponent<SteeringBehaviours>().turnOn(SteeringBehaviours.behaviour_type.pursuit);
        boid.GetComponent<SteeringBehaviours>().leader = leader;
    }

    public override void TearDown()
    {
        
    }
}
