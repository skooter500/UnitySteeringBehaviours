using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BGE.Scenarios
{
    public class SeekScenario : Scenario
    {

        public override string Description()
        {
            return "Seek Demo";
        }

        public override void Start()
        {
            Params.Load("default.txt");

            leader = CreateBoid(new Vector3(-20, 20, 20), leaderPrefab);
            leader.GetComponent<SteeringBehaviours>().SeekEnabled = true;
            leader.GetComponent<SteeringBehaviours>().ObstacleAvoidanceEnabled = true;
            leader.GetComponent<SteeringBehaviours>().PlaneAvoidanceEnabled = true;
            leader.GetComponent<SteeringBehaviours>().seekTargetPos = new Vector3(-30, 30, 80);

            CreateCamFollower(leader, new Vector3(0, 5, -10));

            GroundEnabled(true);
            
        }
    }
}