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

            GameObject aiBoid = CreateBoid(aiPos, boidPrefab);
            aiBoid.AddComponent<StateMachine>();
            aiBoid.GetComponent<StateMachine>().SwicthState(new IdleState(aiBoid));


            leader = CreateBoid(new Vector3(10, 50, 0), leaderPrefab);
            leader.GetComponent<SteeringBehaviours>().maxSpeed = 350;
            leader.AddComponent<StateMachine>();
            leader.GetComponent<StateMachine>().SwicthState(new TeaseState(leader, aiBoid));


            CreateCamFollower(leader, new Vector3(0, 5, -10));
            
            
        }
	}
}
