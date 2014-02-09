using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BGE.Scenarios
{
    class FlockingScenario : Scenario
    {
        public override string Description()
        {
            return "Flocking Demo";
        }

        public override void Start()
        {
            Params.Load("flocking.txt");
            float range = Params.GetFloat("world_range");

            // Create the avoidance boid
            leader = CreateBoid(Utilities.RandomPosition(range), leaderPrefab);
            leader.GetComponent<SteeringBehaviours>().turnOn(SteeringBehaviours.behaviour_type.obstacle_avoidance);
            leader.GetComponent<SteeringBehaviours>().turnOn(SteeringBehaviours.behaviour_type.wander);
            leader.GetComponent<SteeringBehaviours>().turnOn(SteeringBehaviours.behaviour_type.sphere_constrain);

            // Create the boids
            for (int i = 0; i < Params.GetFloat("num_boids"); i++)
            {
                Vector3 pos = Utilities.RandomPosition(range);
                GameObject boid = CreateBoid(pos, boidPrefab);
                boid.GetComponent<SteeringBehaviours>().turnOn(SteeringBehaviours.behaviour_type.separation);
                boid.GetComponent<SteeringBehaviours>().turnOn(SteeringBehaviours.behaviour_type.cohesion);
                boid.GetComponent<SteeringBehaviours>().turnOn(SteeringBehaviours.behaviour_type.alignment);
                boid.GetComponent<SteeringBehaviours>().turnOn(SteeringBehaviours.behaviour_type.wander);
                boid.GetComponent<SteeringBehaviours>().turnOn(SteeringBehaviours.behaviour_type.sphere_constrain);
                boid.GetComponent<SteeringBehaviours>().turnOn(SteeringBehaviours.behaviour_type.obstacle_avoidance);
            }

            // Create some obstacles..
            int numObstacles = 6;
            float dist = (range * 2) / numObstacles;
            float radius = 20.0f;
            for (float x = -range; x < range; x += dist)
            {
                for (float z = -range; z < range; z += dist)
                {
                    CreateObstacle(new Vector3(x, 0, z), radius);
                }
            }

            GroundEnabled(false);

            CreateCamFollower(leader, new Vector3(0, 0, -10));
        }
    }
}