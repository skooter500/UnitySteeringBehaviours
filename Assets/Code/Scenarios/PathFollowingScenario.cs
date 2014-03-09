using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BGE.Scenarios
{
    class PathFollowingScenario : Scenario
    {
        public override string Description()
        {
            return "Path Following Demo";
        }
        static Vector3 initialPos = Vector3.zero;

        public override void Start()
        {
            Params.Load("default.txt");

            leader = CreateBoid(new Vector3(-20, 5, 50), leaderPrefab);

            if (initialPos == Vector3.zero)
            {
                initialPos = leader.transform.position;
            }
            Path path = leader.GetComponent<SteeringBehaviours>().path;
            leader.GetComponent<SteeringBehaviours>().path.Waypoints.Add(initialPos);
            path.Waypoints.Add(initialPos + new Vector3(-50, 0, 80));
            path.Waypoints.Add(initialPos + new Vector3(0, 0, 160));
            path.Waypoints.Add(initialPos + new Vector3(50, 0, 80));
            path.Looped = true;
            path.draw = true;
            leader.GetComponent<SteeringBehaviours>().TurnOffAll();
            leader.GetComponent<SteeringBehaviours>().FollowPathEnabled = true;
            leader.GetComponent<SteeringBehaviours>().ObstacleAvoidanceEnabled = true;

            CreateCamFollower(leader, new Vector3(0, 5, -10));

            GroundEnabled(true);
        }
    }
}