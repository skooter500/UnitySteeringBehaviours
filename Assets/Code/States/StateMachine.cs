using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BGE.States
{
	public class StateMachine:MonoBehaviour
	{
        State currentState;

        void Start()
        {
        }

        public void Update()
        {
            if (currentState != null)
            {
                SteeringManager.PrintMessage("Current state: " + currentState.Description());
                currentState.Update();
            }
        }

        public void SwicthState(State newState)
        {
            if (currentState != null)
            {
                currentState.Exit();
            }

            currentState = newState;
            if (newState != null)
            {
                currentState.Enter();
            }
        }
	}
}
