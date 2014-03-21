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
            leader.GetComponent<SteeringBehaviours>().ArriveEnabled = true;
            leader.GetComponent<SteeringBehaviours>().ObstacleAvoidanceEnabled = true;
            leader.GetComponent<SteeringBehaviours>().SeparationEnabled = true;
            leader.GetComponent<SteeringBehaviours>().PlaneAvoidanceEnabled = true;
            leader.GetComponent<SteeringBehaviours>().seekTargetPos = new Vector3(10, 100, 550);

            // Add some Obstacles
            CreateObstacle(new Vector3(5, 115, 30), 5);
            CreateObstacle(new Vector3(-10, 126, 80), 17);
            CreateObstacle(new Vector3(10, 115, 120), 10);
            CreateObstacle(new Vector3(5, 120, 150), 12);
            CreateObstacle(new Vector3(-2, 125, 200), 20);
            CreateObstacle(new Vector3(-25, 80, 250), 10);
            CreateObstacle(new Vector3(20, 80, 250), 10);
            CreateObstacle(new Vector3(5, 130, 350), 35);

            // Now make a fleet
            int fleetSize = 5;
            float xOff = 12;
            float zOff = -12;
            for (int i = 2; i < fleetSize; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    float z = (i - 1) * zOff;
                    Vector3 offset = new Vector3((xOff * (-i / 2.0f)) + (j * xOff), 0, z);
                    GameObject fleet = CreateBoid(leader.transform.position + offset, boidPrefab);
                    fleet.GetComponent<SteeringBehaviours>().leader = leader;
                    fleet.GetComponent<SteeringBehaviours>().offset = offset;
                    fleet.GetComponent<SteeringBehaviours>().ObstacleAvoidanceEnabled = true;
                    fleet.GetComponent<SteeringBehaviours>().seekTargetPos = new Vector3(0, 0, 450);
                    fleet.GetComponent<SteeringBehaviours>().OffsetPursuitEnabled = true;
                    fleet.GetComponent<SteeringBehaviours>().SeparationEnabled = true;
                    fleet.GetComponent<SteeringBehaviours>().PlaneAvoidanceEnabled = true;
                }
            }

            Vector3 camOffset = new Vector3(0, 5, fleetSize * zOff);
            CreateCamFollower(leader, camOffset);

            GroundEnabled(true);

            
        }

    }
}