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
            leader.GetComponent<SteeringBehaviours>().turnOn(SteeringBehaviours.behaviour_type.arrive);
            leader.GetComponent<SteeringBehaviours>().turnOn(SteeringBehaviours.behaviour_type.obstacle_avoidance);
            leader.GetComponent<SteeringBehaviours>().turnOn(SteeringBehaviours.behaviour_type.wall_avoidance);
            leader.GetComponent<SteeringBehaviours>().seekTargetPos = aiPos + new Vector3(0, 0, 200);

            GameObject aiBoid = CreateBoid(aiPos, boidPrefab);
            aiBoid.AddComponent<StateMachine>();
            aiBoid.GetComponent<StateMachine>().SwicthState(new IdleState(aiBoid));

            CreateCamFollower(leader, new Vector3(0, 5, -10));
            
        }
	}
}
