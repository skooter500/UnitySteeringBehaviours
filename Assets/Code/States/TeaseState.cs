using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BGE.States
{
	public class TeaseState:State
	{
        GameObject teasee;

        public override string Description()
        {
            return "Tease State";
        }

        public TeaseState(GameObject entity, GameObject teasee):base(entity)
        {
            this.teasee = teasee;
        }

        public override void Enter()
        {
            entity.GetComponent<SteeringBehaviours>().TurnOffAll();
            entity.GetComponent<SteeringBehaviours>().PursuitEnabled = true;
            //entity.GetComponent<SteeringBehaviours>().WanderEnabled = true;
            entity.GetComponent<SteeringBehaviours>().target = teasee;
        }

        public override void Update()
        {
            if (Vector3.Distance(entity.transform.position, teasee.transform.position) < 20)
            {
                entity.GetComponent<StateMachine>().SwicthState(new EvadeState(entity, teasee));
            }
        }

        public override void Exit()
        {
            entity.GetComponent<SteeringBehaviours>().TurnOffAll();            
        }
	}
}
