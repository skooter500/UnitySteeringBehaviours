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
            Vector3 leaderPos = new Vector3(-50, 50, 80);
            leader = CreateBoid(leaderPos, leaderPrefab);
            leader.GetComponent<SteeringBehaviours>().
            fighter.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.obstacle_avoidance);
            fighter.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.wall_avoidance);
            fighter.Position = new Vector3(10, 50, 0);
            fighter.TargetPos = aiFighter.Position + new Vector3(-50, 0, -80);

            GameObject aiBoid = CreateBoid(new Vector3(-20, 50, 50), boidPrefab);
            aiBoid.AddComponent<StateMachine>();
            aiBoid.GetComponent<StateMachine>().SwicthState(new IdleState(leader));
            
        }
	}
}
