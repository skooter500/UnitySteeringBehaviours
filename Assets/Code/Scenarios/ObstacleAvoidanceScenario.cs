using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BGE.Scenarios
{
    class ObstacleAvoidanceScenario : Scenario
    {

        public override string Description()
        {
            return "Obstacle Avoidance Demo";
        }

        public override void Start()
        {
            Params.Load("default.txt");

            leader = CreateBoid(new Vector3(10, 120, -20), leaderPrefab);
            leader.GetComponent<SteeringBehaviours>().turnOn(SteeringBehaviours.behaviour_type.arrive);
            leader.GetComponent<SteeringBehaviours>().turnOn(SteeringBehaviours.behaviour_type.obstacle_avoidance);
            leader.GetComponent<SteeringBehaviours>().turnOn(SteeringBehaviours.behaviour_type.separation);
            leader.GetComponent<SteeringBehaviours>().turnOn(SteeringBehaviours.behaviour_type.wall_avoidance);
            leader.GetComponent<SteeringBehaviours>().seekTargetPos = new Vector3(0, 100, 450);

            // Add some Obstacles
            CreateObstacle(new Vector3(0, 120, 10), 4);
            CreateObstacle(new Vector3(-10, 116, 80), 17);
            CreateObstacle(new Vector3(10, 115, 120), 10);
            CreateObstacle(new Vector3(5, 90, 150), 12);
            CreateObstacle(new Vector3(-2, 105, 200), 20);
            CreateObstacle(new Vector3(-25, 80, 250), 10);
            CreateObstacle(new Vector3(20, 80, 250), 10);
            CreateObstacle(new Vector3(5, 70, 320), 35);

            // Now make a fleet
            int fleetSize = 5;
            float xOff = 6;
            float zOff = -6;
            for (int i = 2; i < fleetSize; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    float z = (i - 1) * zOff;
                    Vector3 offset = new Vector3((xOff * (-i / 2.0f)) + (j * xOff), 0, z);
                    GameObject fleet = CreateBoid(leader.transform.position + offset, boidPrefab);
                    fleet.GetComponent<SteeringBehaviours>().leader = leader;
                    fleet.GetComponent<SteeringBehaviours>().offset = offset;
                    fleet.GetComponent<SteeringBehaviours>().turnOn(SteeringBehaviours.behaviour_type.obstacle_avoidance);
                    fleet.GetComponent<SteeringBehaviours>().seekTargetPos = new Vector3(0, 0, 450);
                    fleet.GetComponent<SteeringBehaviours>().turnOn(SteeringBehaviours.behaviour_type.offset_pursuit);
                    fleet.GetComponent<SteeringBehaviours>().turnOn(SteeringBehaviours.behaviour_type.separation);
                    fleet.GetComponent<SteeringBehaviours>().turnOn(SteeringBehaviours.behaviour_type.wall_avoidance);
                }
            }

            Vector3 camOffset = new Vector3(0, 5, fleetSize * zOff);
            CreateCamFollower(leader, camOffset);

            GroundEnabled(true);

        }

    }
}