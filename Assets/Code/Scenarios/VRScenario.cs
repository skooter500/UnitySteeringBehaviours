using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BGE.Scenarios
{
	public class VRScenario:Scenario
	{
        List<GameObject> obstacles = new List<GameObject>();
        List<GameObject> boids = new List<GameObject>();

        public override string Description()
        {
            return "VR Demo";
        }

        private GameObject CreateOffsetBoid(Vector3 offset)
        {
            GameObject boid = CreateBoid(offset, boidPrefab);
            boid.GetComponent<SteeringBehaviours>().leader = leader;
            boid.GetComponent<SteeringBehaviours>().offset = offset;
            boid.GetComponent<SteeringBehaviours>().ObstacleAvoidanceEnabled = true;
            boid.GetComponent<SteeringBehaviours>().OffsetPursuitEnabled = true;
            boid.GetComponent<SteeringBehaviours>().SeparationEnabled = true;
            return boid;
        }

        public override void Start()
        {

            Params.Load("vr.txt");
            float range = 100.0f;
            float halfRange = range / 2.0f;

            int obsCount = 100;
            for (int i = -obsCount ; i < obsCount; i++)
            {
                Vector3 pos = new Vector3();
                pos.x = UnityEngine.Random.Range(- halfRange, halfRange);
                pos.y = UnityEngine.Random.Range(- halfRange, halfRange);
                pos.z = i * range * 2.0f;
                GameObject obstacle = CreateObstacle(pos, UnityEngine.Random.Range(range / 4, range));
                obstacles.Add(obstacle);
            }

            leader = CreateBoid(new Vector3(0, 0, 0), leaderPrefab);
            leader.GetComponentInChildren<MeshRenderer>().enabled = false;
            leader.GetComponent<SteeringBehaviours>().ArriveEnabled = true;
            leader.GetComponent<SteeringBehaviours>().WanderEnabled = true;
            leader.GetComponent<SteeringBehaviours>().ObstacleAvoidanceEnabled = true;
            leader.GetComponent<SteeringBehaviours>().seekTargetPos = new Vector3(10, 0, obsCount * range * 2.0f);

            // Create a fleet of boids
            int fleetSize = 8;
            float xOff = 20;
            float zOff = 20;
            float yOff = 20;

            for (int i = 1; i < fleetSize; i++)
            {
                Vector3 offset;
                offset.x = xOff * i;
                offset.y = 0;
                offset.z = (zOff * 10) -i * zOff;
                GameObject boid = CreateOffsetBoid(offset);
                boid.GetComponent<SteeringBehaviours>().WanderEnabled = true;
                offset.x = -xOff * i;
                CreateOffsetBoid(offset);                
            }

            // Create a flock
            for (int i = 0; i < 100; i++)
            {
                Vector3 pos = Utilities.RandomPosition(range);
                GameObject boid = CreateBoid(pos, boidPrefab);
                boid.GetComponent<SteeringBehaviours>().SeparationEnabled = true;
                boid.GetComponent<SteeringBehaviours>().CohesionEnabled = true;
                boid.GetComponent<SteeringBehaviours>().AlignmentEnabled = true;
                boid.GetComponent<SteeringBehaviours>().WanderEnabled = true;
                boid.GetComponent<SteeringBehaviours>().SphereConstrainEnabled = true;
                boid.GetComponent<SteeringBehaviours>().ObstacleAvoidanceEnabled = true;
                boid.GetComponent<SteeringBehaviours>().maxSpeed = 40.0f;
                boid.GetComponent<SteeringBehaviours>().maxForce = 200.0f;
                boids.Add(boid);
            }

            Vector3 camOffset = new Vector3(0, 5, -5);
            CreateCamFollower(leader, camOffset);
            GroundEnabled(false);
        }

        public override void Update()
        {
            foreach (GameObject boid in boids)
            {
                boid.GetComponent<SteeringBehaviours>().sphereCentre = leader.transform.position;
            }
        }
	}
}
