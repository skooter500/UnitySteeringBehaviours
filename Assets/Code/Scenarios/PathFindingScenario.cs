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

            targetPos = new Vector3(20, 0, 250);
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
            leader.GetComponent<SteeringBehaviours>().FollowPathEnabled = true;

            CreateCamFollower(leader, new Vector3(0, 5, -10));

            GroundEnabled(false);
        }

        public override void Update()
        {
            bool recalculate = false;

            SteeringManager.PrintMessage("Press P to toggle smooth paths");
            SteeringManager.PrintMessage("Press O to toggle 3D paths");

            if (Input.GetKeyDown(KeyCode.P) && ! lastPressed)
            {
                pathFinder.Smooth = !pathFinder.Smooth;
                recalculate = true;
            }

            if (Input.GetKeyDown(KeyCode.O) && !lastPressed)
            {
                pathFinder.isThreeD = !pathFinder.isThreeD;
                recalculate = true;
            }

            GameObject camera = (GameObject) GameObject.FindGameObjectWithTag("MainCamera");

            if (Input.GetMouseButton(0))
            {
                Plane worldPlane = new Plane(new Vector3(0, 1, 0), 0);
                Ray ray = new Ray(camera.transform.position, camera.transform.forward);
                float distance = 0;
                if (worldPlane.Raycast(ray, out distance))
                {
                    targetPos = camera.transform.position + (camera.transform.forward * distance);
                }
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
                    leader.GetComponent<SteeringBehaviours>().FollowPathEnabled = true;
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

            if (pathFinder.message != "")
            {
                SteeringManager.PrintMessage(pathFinder.message);
            }

            base.Update();
        }
	}
}
