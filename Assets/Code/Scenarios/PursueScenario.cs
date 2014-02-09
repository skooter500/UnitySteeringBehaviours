using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Steering.Scenarios
{
    class PursueScenario : Scenario
    {
        public override string Description()
        {
            return "Pursuit Demo";
        }
        public override void SetUp()
        {
            Params.Load("default.txt");
            leader = CreateBoid(new Vector3(10, 5, 10), leaderPrefab);
            leader.GetComponent<SteeringBehaviours>().turnOn(SteeringBehaviours.behaviour_type.seek);
            leader.GetComponent<SteeringBehaviours>().seekTargetPos = new Vector3(100, 5, 100);

            GameObject boid = CreateBoid(new Vector3(25, 5, 50), boidPrefab);
            boid.GetComponent<SteeringBehaviours>().turnOn(SteeringBehaviours.behaviour_type.pursuit);
            boid.GetComponent<SteeringBehaviours>().leader = leader;

            CreateCamFollower(boid, new Vector3(0, 5, -10));
        }
    }
}