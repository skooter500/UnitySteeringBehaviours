using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BGE.Scenarios
{
	public class PathFindingScenario:Scenario
	{
        private PathFinder pathFinder;
        
        Vector3 targetPos;
        Vector3 startPos;

        bool lastPressed = false;

        public override string Description()
        {
            return "Path Finding Demo";
        }

        public override void Start()
        {
            Params.Load("default.txt");

            targetPos = new Vector3(20, 0, 150);
            startPos = new Vector3(10, 0, -20);

            leader = CreateBoid(startPos, boidPrefab);

            CreateObstacle(new Vector3(15, 0, 10), 10);
            CreateObstacle(new Vector3(5, 0, 50), 12);
            CreateObstacle(new Vector3(15, 0, 70), 5);

            pathFinder = new PathFinder();

            Path path = pathFinder.FindPath(startPos, targetPos);
            path.Looped = false;
            path.draw = true;
            leader.GetComponent<SteeringBehaviours>().path = path;
            leader.GetComponent<SteeringBehaviours>().turnOn(SteeringBehaviours.behaviour_type.follow_path);

            GroundEnabled(false);
        }

        public override void Update()
        {
            bool recalculate = false;

            SteeringManager.PrintMessage("Press P to smooth path");
            //SteeringManager.PrintMessage("Press O for 3D path");

            if (Input.GetKeyDown(KeyCode.P) && ! lastPressed)
            {
                pathFinder.Smooth = !pathFinder.Smooth;
                recalculate = true;
            }

            if (Input.GetKeyDown(KeyCode.O) && !lastPressed)
            {
                pathFinder.IsThreeD = !pathFinder.IsThreeD;
                recalculate = true;
            }

            GameObject camera = (GameObject) GameObject.FindGameObjectWithTag("MainCamera");

            if (Input.GetMouseButton(0))
            {
                targetPos = camera.transform.position + camera.transform.forward * 100.0f;
                targetPos.y = 0;
                recalculate = true;
            }

            if (recalculate)
            {
                Path path = pathFinder.FindPath(startPos, targetPos);
                if (path.Waypoints.Count == 0)
                {
                    leader.GetComponent<SteeringBehaviours>().turnOffAll();
                }
                else
                {
                    leader.GetComponent<SteeringBehaviours>().turnOn(SteeringBehaviours.behaviour_type.follow_path);
                }
                leader.GetComponent<SteeringBehaviours>().path = path;
                leader.GetComponent<SteeringBehaviours>().path.draw = true;
                leader.GetComponent<SteeringBehaviours>().path.Next = 0;
            }

            if (Input.anyKeyDown)
            {
                lastPressed = true;
            }
            else
            {
                lastPressed = false;
            }

            base.Update();
        }
	}
}
