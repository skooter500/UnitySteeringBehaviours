using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BGE.Scenarios
{
    class PursueScenario : Scenario
    {
        public override string Description()
        {
            return "Pursuit Demo";
        }
        public override void Start()
        {
            Params.Load("default.txt");
            leader = CreateBoid(new Vector3(10, 5, 60), leaderPrefab);
            leader.GetComponent<SteeringBehaviours>().RandomWalkEnabled = true;
            leader.GetComponent<SteeringBehaviours>().seekTargetPos = new Vector3(100, 5, 100);

            GameObject boid = CreateBoid(new Vector3(25, 5, 50), boidPrefab);
            boid.GetComponent<SteeringBehaviours>().PursuitEnabled = true;
            boid.GetComponent<SteeringBehaviours>().leader = leader;

            CreateCamFollower(boid, new Vector3(0, 5, -10));

            
        }
    }
}