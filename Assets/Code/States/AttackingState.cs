using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

namespace BGE.States
{
    class AttackingState:State
    {
        float timeShot = 0.25f;

        public override string Description()
        {
            return "Attacking State";
        }

        public AttackingState(GameObject entity):base(entity)
        {
        }

        public override void Enter()
        {
            entity.GetComponent<SteeringBehaviours>().TurnOffAll();
            entity.GetComponent<SteeringBehaviours>().OffsetPursuitEnabled = true;
            entity.GetComponent<SteeringBehaviours>().ObstacleAvoidanceEnabled = true;
            entity.GetComponent<SteeringBehaviours>().offset = new Vector3(0, 0, 5);
            entity.GetComponent<SteeringBehaviours>().leader = SteeringManager.Instance().currentScenario.leader;
        }

        public override void Exit()
        {
        }

        public override void Update()
        {
            float range = 50.0f;
            timeShot += Time.deltaTime;
            float fov = Mathf.PI / 4.0f;
            // Can I see the leader?
            GameObject leader = SteeringManager.Instance().currentScenario.leader;
            if ((leader.transform.position - entity.transform.position).magnitude > range)
            {
                entity.GetComponent<StateMachine>().SwicthState(new IdleState(entity));
            }
            else
            {
                float angle;
                Vector3 toEnemy = (leader.transform.position - entity.transform.position);
                toEnemy.Normalize();
                angle = (float) Math.Acos(Vector3.Dot(toEnemy, entity.transform.forward));
                if (angle < fov)
                {
                    if (timeShot > 0.25f)
                    {
                        GameObject lazer = new GameObject();
                        lazer.AddComponent<Lazer>();
                        lazer.transform.position = entity.transform.position;
                        lazer.transform.forward = entity.transform.forward;
                        timeShot = 0.0f;
                    }
                }
            }
        }

    }
}
