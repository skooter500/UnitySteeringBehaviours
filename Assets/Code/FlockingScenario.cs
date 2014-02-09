using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class FlockingScenario:Scenario
{
    public override string Description()
    {
        return "Flocking Demo";
    }

    public override void SetUp()
    {
        Params.Load("flocking.txt");
        float range = Params.GetFloat("world_range");
            
        leader = CreateBoid(Utilities.RandomPosition(range),  leaderPrefab);
        leader.GetComponent<SteeringBehaviours>().turnOn(SteeringBehaviours.behaviour_type.obstacle_avoidance);
        leader.GetComponent<SteeringBehaviours>().turnOn(SteeringBehaviours.behaviour_type.wander);
        leader.GetComponent<SteeringBehaviours>().turnOn(SteeringBehaviours.behaviour_type.pursuit);
        leader.GetComponent<SteeringBehaviours>().turnOn(SteeringBehaviours.behaviour_type.sphere_constrain);

        CreateCamFollower(leader, new Vector3(0, 0, -10));
    }
}
