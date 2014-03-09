using System;
using System.Collections.Generic;
using UnityEngine;
using BGE;

namespace BGE.States
{
    public class IdleState:State
    {
        static Vector3 initialPos = Vector3.zero;

        public override string Description()
        {
            return "Idle State";
        }

        public IdleState(GameObject entity):base(entity)
        {
        }

        public override void Enter()
        {
            if (initialPos == Vector3.zero)
            {
                initialPos = entity.transform.position;
            }
            entity.GetComponent<SteeringBehaviours>().path.Waypoints.Add(initialPos);
            entity.GetComponent<SteeringBehaviours>().path.Waypoints.Add(initialPos + new Vector3(-50, 0, 80));
            entity.GetComponent<SteeringBehaviours>().path.Waypoints.Add(initialPos + new Vector3(0, 0, 160));
            entity.GetComponent<SteeringBehaviours>().path.Waypoints.Add(initialPos + new Vector3(50, 0, 80));
            entity.GetComponent<SteeringBehaviours>().path.Looped = true;            
            entity.GetComponent<SteeringBehaviours>().path.draw = true;
            entity.GetComponent<SteeringBehaviours>().TurnOffAll();
            entity.GetComponent<SteeringBehaviours>().FollowPathEnabled = true;
            entity.GetComponent<SteeringBehaviours>().ObstacleAvoidanceEnabled = true;
        }
        public override void Exit()
        {
            entity.GetComponent<SteeringBehaviours>().path.Waypoints.Clear();
        }

        public override void Update()
        {
            float range = 50.0f;           
            // Can I see the leader?
            GameObject leader = SteeringManager.Instance().currentScenario.leader;
            if ((leader.transform.position - entity.transform.position).magnitude < range)
            {
                // Is the leader inside my FOV
                entity.GetComponent<StateMachine>().SwicthState(new AttackingState(entity));
            }
        }
    }
}
