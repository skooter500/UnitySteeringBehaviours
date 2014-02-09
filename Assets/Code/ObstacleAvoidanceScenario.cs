using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class ObstacleAvoidanceScenario:Scenario
{
    System.Random random = new System.Random(DateTime.Now.Millisecond);

    public override string Description()
    {
        return "Obstacle Avoidance Demo";
    }

    public void CreateObstacle(Vector3 position, float radius)
    {
        GameObject o;

        o = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        o.tag = "obstacle";
        o.renderer.material.color = new Color((float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble());
        o.transform.localScale = new Vector3(radius * 2, radius * 2, radius * 2);
        o.transform.position = position;
    }

    public override void SetUp()
    {
        Params.Load("default.txt");

        leader = (GameObject)GameManager.Instantiate(leader);
        leader.tag = "boid";
        leader.AddComponent<SteeringBehaviours>();
        leader.transform.position = new Vector3(10, 120, -20);
        leader.GetComponent<SteeringBehaviours>().turnOn(SteeringBehaviours.behaviour_type.arrive);
        leader.GetComponent<SteeringBehaviours>().turnOn(SteeringBehaviours.behaviour_type.obstacle_avoidance);
        //leader.GetComponent<SteeringBehaviours>().turnOn(SteeringBehaviours.behaviour_type.separation);
        //leader.GetComponent<SteeringBehaviours>().turnOn(SteeringBehaviours.behaviour_type.wall_avoidance);
        leader.GetComponent<SteeringBehaviours>().seekTargetPos = new Vector3(0, 100, 450);

        // Add some Obstacles
        CreateObstacle(new Vector3(0, 100, 10), 4);
        CreateObstacle(new Vector3(-10, 116, 80), 17);
        CreateObstacle(new Vector3(10, 115, 120), 10);
        CreateObstacle(new Vector3(5, 90, 150), 12);
        CreateObstacle(new Vector3(-2, 105, 200), 20);
        CreateObstacle(new Vector3(-25, 80, 250), 10);
        CreateObstacle(new Vector3(20, 80, 250), 10);
        CreateObstacle(new Vector3(-10, 70, 300), 35);
    
        // Now make a fleet
        int fleetSize = 5;
        float xOff = 6;
        float zOff = -6;
        for (int i = 2; i < fleetSize; i++)
        {
            for (int j = 0; j < i; j++)
            {
                float z = (i - 1) * zOff;
                GameObject fleet = (GameObject) GameObject.Instantiate(boid);
                fleet.tag = "boid";
                fleet.AddComponent<SteeringBehaviours>();        
                fleet.GetComponent<SteeringBehaviours>().leader = leader;                
                fleet.GetComponent<SteeringBehaviours>().offset = new Vector3((xOff * (-i / 2.0f)) + (j * xOff), 0, z);
                fleet.transform.position = leader.transform.position + fleet.GetComponent<SteeringBehaviours>().offset;
                fleet.GetComponent<SteeringBehaviours>().turnOn(SteeringBehaviours.behaviour_type.obstacle_avoidance);
                fleet.GetComponent<SteeringBehaviours>().seekTargetPos = new Vector3(0, 0, 450);
                fleet.GetComponent<SteeringBehaviours>().turnOn(SteeringBehaviours.behaviour_type.offset_pursuit);
                fleet.GetComponent<SteeringBehaviours>().turnOn(SteeringBehaviours.behaviour_type.separation);
                fleet.GetComponent<SteeringBehaviours>().turnOn(SteeringBehaviours.behaviour_type.wall_avoidance);
            }
        }

        GameObject camFighter = new GameObject();
        camFighter.tag = "camFollower";
        camFighter.AddComponent<SteeringBehaviours>();
        camFighter.GetComponent<SteeringBehaviours>().leader = leader;
        camFighter.GetComponent<SteeringBehaviours>().offset = new Vector3(0, 5, fleetSize * zOff);
        camFighter.transform.position = new Vector3(0, 115, fleetSize * zOff);
        camFighter.GetComponent<SteeringBehaviours>().turnOn(SteeringBehaviours.behaviour_type.offset_pursuit);
        //camFighter.GetComponent<SteeringBehaviours>().turnOn(SteeringBehaviours.behaviour_type.wall_avoidance);
        camFighter.GetComponent<SteeringBehaviours>().turnOn(SteeringBehaviours.behaviour_type.obstacle_avoidance);
        GameManager.Instance().camFighter = camFighter;
        GameManager.Instance().ground.renderer.enabled = true;
        GameObject.FindGameObjectWithTag("MainCamera").transform.position = camFighter.transform.position;          
    }

    public override void TearDown()
    {
        
    }
}
