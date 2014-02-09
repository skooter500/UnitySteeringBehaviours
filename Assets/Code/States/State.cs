using System;
using UnityEngine;

namespace Steering.States
{
    public abstract class State
    {
        protected GameObject entity;

        public State(GameObject entity)
        {
            this.entity = entity;
        }

        public GameObject Entity
        {
            get { return entity; }
            set { entity = value; }
        }
        public abstract void Enter();
        public abstract void Exit();

        public abstract void Update(float gameTime);
    }
}
