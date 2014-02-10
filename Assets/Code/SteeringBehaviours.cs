using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace BGE
{
    public class SteeringBehaviours : MonoBehaviour
    {

        public Vector3 force;
        public Vector3 velocity;
        public Vector3 acceleration;

        public float myRadius;

        public float mass;

        public enum CalculationMethods { WeightedTruncatedSum, WeightedTruncatedRunningSumWithPrioritisation, PrioritisedDithering };
        CalculationMethods calculationMethod;

        List<GameObject> tagged = new List<GameObject>();
        List<Vector3> Feelers = new List<Vector3>();

        // Values required to implement certain behaviours
        public GameObject target; // required for evade
        public GameObject leader; // required for offset pursuit
        private Vector3 wanderTargetPos;
        public Vector3 seekTargetPos;
        public Vector3 offset;
        private Vector3 randomWalkTarget;
        

        Color debugLineColour = Color.cyan;

        float timeDelta;

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

        public Vector3 Calculate()
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
                force = Seek(seekTargetPos) * Params.GetWeight("seek_weight");
                if (!accumulateForce(ref steeringForce, force))
                {
                    return steeringForce;
                }
            }

            if (isOn(behaviour_type.arrive))
            {
                force = Arrive(seekTargetPos) * Params.GetWeight("arrive_weight");
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
                force = OffsetPursuit(offset) * Params.GetWeight("offset_pursuit_weight");
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

        void Update()
        {
            float smoothRate;
            force = Calculate();
            SteeringBehaviours.checkNaN(force);
            Vector3 newAcceleration = force / mass;

            timeDelta = Time.deltaTime * Params.GetFloat(Params.TIME_MODIFIER_KEY);

            if (timeDelta > 0.0f)
            {
                smoothRate = Utilities.Clip(9.0f * timeDelta, 0.15f, 0.4f) / 2.0f;
                Utilities.BlendIntoAccumulator(smoothRate, newAcceleration, ref acceleration);
            }

            velocity += acceleration * timeDelta;

            float speed = velocity.magnitude;
            if (speed > Params.GetFloat("max_speed"))
            {
                velocity.Normalize();
                velocity *= Params.GetFloat("max_speed");
            }
            transform.position += velocity * timeDelta;


            // the length of this global-upward-pointing vector controls the vehicle's
            // tendency to right itself as it is rolled over from turning acceleration
            Vector3 globalUp = new Vector3(0, 0.2f, 0);
            // acceleration points toward the center of local path curvature, the
            // length determines how much the vehicle will roll while turning
            Vector3 accelUp = acceleration * 0.05f;
            // combined banking, sum of UP due to turning and global UP
            Vector3 bankUp = accelUp + globalUp;
            // blend bankUp into vehicle's UP basis vector
            smoothRate = timeDelta * 3.0f;
            Vector3 tempUp = transform.up;
            Utilities.BlendIntoAccumulator(smoothRate, bankUp, ref tempUp);

            if (speed > 0.0001f)
            {
                transform.forward = velocity;
                transform.forward.Normalize();
                transform.LookAt(transform.position + transform.forward, tempUp);
                // Apply damping
                velocity *= 0.99f;
            }
        }

        #endregion

        #region Behaviours

        Vector3 Seek(Vector3 targetPos)
        {
            return Vector3.zero;
        }

        Vector3 Evade()
        {
            return Vector3.zero;
        }

        Vector3 ObstacleAvoidance()
        {
            return Vector3.zero;;
        }

        Vector3 OffsetPursuit(Vector3 offset)
        {
            return Vector3.zero;
        }

        Vector3 Pursue()
        {
            return Vector3.zero;
        }

        Vector3 Flee(Vector3 targetPos)
        {
            return Vector3.zero;
        }

        Vector3 RandomWalk()
        {
            return Vector3.zero;
        }



        Vector3 Wander()
        {
            return Vector3.zero;
        }

        public Vector3 WallAvoidance()
        {
            return Vector3.zero;
        }

        public void DrawFeelers()
        {
            foreach (Vector3 feeler in Feelers)
            {
                Debug.DrawLine(transform.position, feeler, debugLineColour);
            }
        }

        public Vector3 Arrive(Vector3 target)
        {
            return Vector3.zero;
        }

        private Vector3 FollowPath()
        {
            return Vector3.zero;
        }

        public Vector3 SphereConstrain(float radius)
        {
            return Vector3.zero;
        }

        #endregion

        #region Flocking
        private int TagNeighboursSimple(float inRange)
        {
            tagged.Clear();

            GameObject[] steerables = GameObject.FindGameObjectsWithTag("boid");
            foreach (GameObject steerable in steerables)
            {
                if (steerable != gameObject)
                {
                    if ((transform.position - steerable.transform.position).magnitude < inRange)
                    {
                        tagged.Add(steerable);
                    }
                }
            }
            return tagged.Count;
        }

        public Vector3 Separation()
        {
            return Vector3.zero;
        }

        public Vector3 Cohesion()
        {
            return Vector3.zero;
        }

        public Vector3 Alignment()
        {
            return Vector3.zero;

        }
        #endregion Flocking

        // Use this for initialization
        void Start()
        {
            myRadius = 5.0f;
        }

        public SteeringBehaviours()
        {
            force = Vector3.zero;
            velocity = Vector3.zero;
            mass = 1.0f;
            flags = 0;
            calculationMethod = CalculationMethods.WeightedTruncatedRunningSumWithPrioritisation;
            target = null;
            leader = null;
        }
    }
}
