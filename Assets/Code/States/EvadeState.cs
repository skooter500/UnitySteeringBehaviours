using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BGE.States
{
	class EvadeState:State
	{
        GameObject teasee;
        public override string Description()
        {
            return "Evade State";
        }

        public EvadeState(GameObject entity, GameObject teasee):base(entity)
        {
            this.teasee = teasee;
        }

        public override void Enter()
        {
            entity.GetComponent<SteeringBehaviours>().TurnOffAll();
            entity.GetComponent<SteeringBehaviours>().EvadeEnabled = true;
            entity.GetComponent<SteeringBehaviours>().WanderEnabled = true;

            entity.GetComponent<SteeringBehaviours>().target = teasee;
        }

        public override void Update()
        {
            if (Vector3.Distance(entity.transform.position, teasee.transform.position) > 200)
            {
                entity.GetComponent<StateMachine>().SwicthState(new TeaseState(entity, teasee));
            }
        }

        public override void Exit()
        {
            entity.GetComponent<SteeringBehaviours>().TurnOffAll();            
        }
	}
}
