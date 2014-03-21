using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using BGE.States;

namespace BGE.Scenarios
{
	class StateMachineScenario:Scenario
	{
        public override string Description()
        {
            return "State Machine Demo";
        }

        public override void Start()
        {
            Params.Load("default.txt");
            Vector3 aiPos = new Vector3(-20, 50, 50);

            leader = CreateBoid(new Vector3(10, 50, 0), leaderPrefab);
            leader.GetComponent<SteeringBehaviours>().ArriveEnabled = true;
            leader.GetComponent<SteeringBehaviours>().ObstacleAvoidanceEnabled = true;
            leader.GetComponent<SteeringBehaviours>().PlaneAvoidanceEnabled = true;
            leader.GetComponent<SteeringBehaviours>().seekTargetPos = aiPos + new Vector3(0, 0, 200);
            leader.GetComponent<SteeringBehaviours>().maxSpeed = 250;

            GameObject aiBoid = CreateBoid(aiPos, boidPrefab);
            aiBoid.AddComponent<StateMachine>();
            aiBoid.GetComponent<StateMachine>().SwicthState(new IdleState(aiBoid));

            CreateCamFollower(leader, new Vector3(0, 5, -10));
            
            
        }
	}
}
