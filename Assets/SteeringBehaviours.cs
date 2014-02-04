using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class SteeringBehaviours : MonoBehaviour {

    public Vector3 force;
    public Vector3 velocity;

    public Vector3 targetPos;

    public float mass;

    public float maxSpeed = 20;
    public float maxForce = 10;

    public enum CalculationMethods { WeightedTruncatedSum, WeightedTruncatedRunningSumWithPrioritisation, PrioritisedDithering };
    CalculationMethods calculationMethod;

    List<GameObject> tagged = new List<GameObject>();
    List<Vector3> Feelers = new List<Vector3>();

    // Special Game Objects required to implement certain behaviours
    GameObject target; // required for evade
    GameObject leader; // required for offset pursuit

    #region Flags
    public enum behaviour_type
    {

        none = 0x00000,
        seek = 0x00002,
        flee = 0x00004,
        arrive = 0x00008,
        wander = 0x00010,
        cohesion = 0x00020,
        separation = 0x00040,
        alignment = 0x00080,
        obstacle_avoidance = 0x00100,
        wall_avoidance = 0x00200,
        follow_path = 0x00400,
        pursuit = 0x00800,
        evade = 0x01000,
        interpose = 0x02000,
        hide = 0x04000,
        flock = 0x08000,
        offset_pursuit = 0x10000,
        sphere_constrain = 0x20000,
        random_walk = 0x40000,
    };

    int flags;

    public bool isOn(behaviour_type behaviour)
    {
        return ((flags & (int)behaviour) == (int)behaviour);
    }

    public void turnOn(behaviour_type behaviour)
    {
        flags |= ((int)behaviour);
    }

    public void turnOffAll()
    {
        flags = (int)SteeringBehaviours.behaviour_type.none;
    }
    #endregion

    #region Utilities
    static System.Random random = new System.Random(DateTime.Now.Millisecond);

    public static float RandomClamped()
    {
        return 1.0f - ((float)random.NextDouble() * 2.0f);
    }   

    static public bool checkNaN(ref Vector3 v, Vector3 def)
    {
        if (float.IsNaN(v.x))
        {
            Debug.LogError("Nan");
            v = def;
            return true;
        }
        if (float.IsNaN(v.y))
        {
            Debug.LogError("Nan");
            v = def;
            return true;
        }
        if (float.IsNaN(v.z))
        {
            Debug.LogError("Nan");
            v = def;
            return true;
        }
        return false;
    }

    static public bool checkNaN(Vector3 v)
    {
        if (float.IsNaN(v.x))
        {
            System.Console.WriteLine("Nan");
            return true;
        }
        if (float.IsNaN(v.y))
        {
            System.Console.WriteLine("Nan");
            return true;
        }
        if (float.IsNaN(v.z))
        {
            System.Console.WriteLine("Nan");
            return true;
        }
        return false;
    }

    private void makeFeelers()
    {
        Feelers.Clear();
        float feelerDistance = 20.0f;
        // Make the forward feeler
        Vector3 newFeeler = Vector3.forward * feelerDistance;
        newFeeler = transform.TransformPoint(newFeeler);
        Feelers.Add(newFeeler);

        newFeeler = Vector3.forward * feelerDistance;
        newFeeler = Quaternion.AngleAxis(45, Vector3.up) * newFeeler;
        newFeeler = transform.TransformPoint(newFeeler);
        Feelers.Add(newFeeler);

        newFeeler = Vector3.forward * feelerDistance;
        newFeeler = Quaternion.AngleAxis(-45, Vector3.up) * newFeeler;
        newFeeler = transform.TransformPoint(newFeeler);
        Feelers.Add(newFeeler);

        newFeeler = Vector3.forward * feelerDistance;
        newFeeler = Quaternion.AngleAxis(45, Vector3.right) * newFeeler;
        newFeeler = transform.TransformPoint(newFeeler);
        Feelers.Add(newFeeler);

        newFeeler = Vector3.forward * feelerDistance;
        newFeeler = Quaternion.AngleAxis(-45, Vector3.right) * newFeeler;
        newFeeler = transform.TransformPoint(newFeeler);        
        Feelers.Add(newFeeler);
    }
    #endregion
    #region Integration

    private bool accumulateForce(ref Vector3 runningTotal, Vector3 force)
    {
        float soFar = runningTotal.magnitude;

        float remaining = Params.GetFloat("max_force") - soFar;
        if (remaining <= 0)
        {
            return false;
        }

        float toAdd = force.magnitude;


        if (toAdd < remaining)
        {
            runningTotal += force;
        }
        else
        {
            runningTotal += Vector3.Normalize(force) * remaining;
        }
        return true;
    }

    public Vector3 calculate()
    {
        if (calculationMethod == CalculationMethods.WeightedTruncatedRunningSumWithPrioritisation)
        {
            return calculateWeightedPrioritised();
        }

        return Vector3.zero;
    }

    private Vector3 calculateWeightedPrioritised()
    {
        Vector3 force = Vector3.zero;
        Vector3 steeringForce = Vector3.zero;


        if (isOn(behaviour_type.obstacle_avoidance))
        {
            force = ObstacleAvoidance() * Params.GetWeight("obstacle_avoidance_weight");
            if (!accumulateForce(ref steeringForce, force))
            {
                return steeringForce;
            }
        }
        checkNaN(force);
        if (isOn(behaviour_type.wall_avoidance))
        {
            force = WallAvoidance() * Params.GetWeight("wall_avoidance_weight");
            if (!accumulateForce(ref steeringForce, force))
            {
                return steeringForce;
            }
        }

        if (isOn(behaviour_type.sphere_constrain))
        {
            force = SphereConstrain(Params.GetFloat("world_range")) * Params.GetWeight("sphere_constrain_weight");
            if (!accumulateForce(ref steeringForce, force))
            {
                return steeringForce;
            }
        }

        if (isOn(behaviour_type.evade))
        {
            force = Evade() * Params.GetWeight("evade_weight");
            if (!accumulateForce(ref steeringForce, force))
            {
                return steeringForce;
            }
        }

        int tagged = 0;
        if (isOn(behaviour_type.separation) || isOn(behaviour_type.cohesion) || isOn(behaviour_type.alignment))
        {
            tagged = TagNeighboursSimple(Params.GetFloat("tag_range"));
        }

        if (isOn(behaviour_type.separation) && (tagged > 0))
        {
            force = Separation() * Params.GetWeight("separation_weight");
            if (!accumulateForce(ref steeringForce, force))
            {
                return steeringForce;
            }
        }

        if (isOn(behaviour_type.alignment) && (tagged > 0))
        {
            force = Alignment() * Params.GetWeight("alignment_weight");
            if (!accumulateForce(ref steeringForce, force))
            {
                return steeringForce;
            }
        }

        if (isOn(behaviour_type.cohesion) && (tagged > 0))
        {
            force = Cohesion() * Params.GetWeight("cohesion_weight");
            if (!accumulateForce(ref steeringForce, force))
            {
                return steeringForce;
            }
        }

        if (isOn(behaviour_type.seek))
        {
            force = Seek(fighter.TargetPos) * Params.GetWeight("seek_weight");
            if (!accumulateForce(ref steeringForce, force))
            {
                return steeringForce;
            }
        }

        if (isOn(behaviour_type.arrive))
        {
            force = Arrive(fighter.TargetPos) * Params.GetWeight("arrive_weight");
            if (!accumulateForce(ref steeringForce, force))
            {
                return steeringForce;
            }
        }

        if (isOn(behaviour_type.wander))
        {
            force = Wander() * Params.GetWeight("wander_weight");
            if (!accumulateForce(ref steeringForce, force))
            {
                return steeringForce;
            }
        }

        if (isOn(behaviour_type.pursuit))
        {
            force = Pursue() * Params.GetWeight("pursuit_weight");
            if (!accumulateForce(ref steeringForce, force))
            {
                return steeringForce;
            }
        }

        if (isOn(behaviour_type.offset_pursuit))
        {
            force = OffsetPursuit(fighter.offset) * Params.GetWeight("offset_pursuit_weight");
            if (!accumulateForce(ref steeringForce, force))
            {
                return steeringForce;
            }
        }

        if (isOn(behaviour_type.follow_path))
        {
            force = FollowPath() * Params.GetWeight("follow_path_weight");
            if (!accumulateForce(ref steeringForce, force))
            {
                return steeringForce;
            }
        }

        if (isOn(behaviour_type.random_walk))
        {
            force = RandomWalk() * Params.GetWeight("random_walk_weight");
            if (!accumulateForce(ref steeringForce, force))
            {
                return steeringForce;
            }
        }

        return steeringForce;
    }
    #endregion

    #region Behaviours
    Vector3 Seek(Vector3 targetPos)
    {
        Vector3 desiredVelocity;

        desiredVelocity = targetPos - transform.position;
        desiredVelocity.Normalize();
        desiredVelocity *= maxSpeed;

        return (desiredVelocity - velocity);
    }
    #endregion

    // Use this for initialization
	void Start () {
	
	}
	
	public SteeringBehaviours()
    {
        force = Vector3.zero;
        velocity = Vector3.zero;
        mass = 10.0f;

        GameObject sphere = GameObject.FindGameObjectWithTag("sphere");
        targetPos = sphere.transform.position;    
    }

    Vector3 Calculate()
    {
        return Seek(targetPos);
    }

    void Update()
    {       
        Vector3 acceleration = Calculate() / mass;
        velocity += acceleration * Time.deltaTime;
        gameObject.transform.position += velocity * Time.deltaTime;

        force = Vector3.zero;

        if (velocity.magnitude > 0.01f)
        {
            gameObject.transform.forward = Vector3.Normalize(velocity);
        }

        velocity *= 0.99f;
    }
}
