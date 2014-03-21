using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BGE.Scenarios
{
    class WanderScenario : Scenario
    {
        public override string Description()
        {
            return "Wander Demo";
        }

        public override void Start()
        {
            Params.Load("default.txt");
            leader = CreateBoid(new Vector3(-20, 50, 20), leaderPrefab);
            leader.GetComponent<SteeringBehaviours>().WanderEnabled = true;
            leader.GetComponent<SteeringBehaviours>().SphereConstrainEnabled = true;

            CreateCamFollower(leader, new Vector3(0, 5, -10));

            GroundEnabled(false);

            
        }
    }
}