using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using BGE.Geom;

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
        public Path path = new Path();
        public float maxSpeed;

        Color debugLineColour = Color.cyan;

        float timeDelta;

        #region Flags

        public void turnOffAll()
        {
            SeekEnabled= false;
            FleeEnabled= false;
            ArriveEnabled= false;
            WanderEnabled= false;
            CohesionEnabled= false;
            SeparationEnabled= false;
            AlignmentEnabled= false;
            ObstacleAvoidanceEnabled= false;
            PlaneAvoidanceEnabled= false;
            FollowPathEnabled= false;
            PursuitEnabled= false;
            EvadeEnabled= false;
            InterposeEnabled= false;
            HideEnabled= false;
            OffsetPursuitEnabled= false;
            SphereConstrainEnabled= false;
            RandomWalkEnabled= false;
        }

        public bool SeekEnabled;
        public bool FleeEnabled;
        public bool ArriveEnabled;
        public bool WanderEnabled;
        public bool CohesionEnabled;
        public bool SeparationEnabled;
        public bool AlignmentEnabled;
        public bool ObstacleAvoidanceEnabled;
        public bool PlaneAvoidanceEnabled;
        public bool FollowPathEnabled;
        public bool PursuitEnabled;
        public bool EvadeEnabled;
        public bool InterposeEnabled;
        public bool HideEnabled;
        public bool OffsetPursuitEnabled;
        public bool SphereConstrainEnabled;
        public bool RandomWalkEnabled;
#endregion

#region Utilities        
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

            if (ObstacleAvoidanceEnabled)
            {
                force = ObstacleAvoidance() * Params.GetWeight("obstacle_avoidance_weight");

                if (!accumulateForce(ref steeringForce, force))
                {
                    return steeringForce;
                }
            }


            Utilities.checkNaN(force);
            if (PlaneAvoidanceEnabled)
            {
                force = WallAvoidance() * Params.GetWeight("wall_avoidance_weight");
                if (!accumulateForce(ref steeringForce, force))
                {
                    return steeringForce;
                }
            }

            if (SphereConstrainEnabled)
            {
                force = SphereConstrain(Params.GetFloat("world_range")) * Params.GetWeight("sphere_constrain_weight");
                if (!accumulateForce(ref steeringForce, force))
                {
                    return steeringForce;
                }
            }

            if (EvadeEnabled)
            {
                force = Evade() * Params.GetWeight("evade_weight");
                if (!accumulateForce(ref steeringForce, force))
                {
                    return steeringForce;
                }
            }

            int tagged = 0;
            if (SeparationEnabled || CohesionEnabled || AlignmentEnabled)
            {
                tagged = TagNeighboursSimple(Params.GetFloat("tag_range"));
            }

            if (SeparationEnabled && (tagged > 0))
            {
                force = Separation() * Params.GetWeight("separation_weight");
                if (!accumulateForce(ref steeringForce, force))
                {
                    return steeringForce;
                }
            }

            if (AlignmentEnabled && (tagged > 0))
            {
                force = Alignment() * Params.GetWeight("alignment_weight");
                if (!accumulateForce(ref steeringForce, force))
                {
                    return steeringForce;
                }
            }

            if (CohesionEnabled && (tagged > 0))
            {
                force = Cohesion() * Params.GetWeight("cohesion_weight");
                if (!accumulateForce(ref steeringForce, force))
                {
                    return steeringForce;
                }
            }

            if (SeekEnabled)
            {
                force = Seek(seekTargetPos) * Params.GetWeight("seek_weight");
                if (!accumulateForce(ref steeringForce, force))
                {
                    return steeringForce;
                }
            }

            if (ArriveEnabled)
            {
                force = Arrive(seekTargetPos) * Params.GetWeight("arrive_weight");
                if (!accumulateForce(ref steeringForce, force))
                {
                    return steeringForce;
                }
            }

            if (WanderEnabled)
            {
                force = Wander() * Params.GetWeight("wander_weight");
                if (!accumulateForce(ref steeringForce, force))
                {
                    return steeringForce;
                }
            }

            if (PursuitEnabled)
            {
                force = Pursue() * Params.GetWeight("pursuit_weight");
                if (!accumulateForce(ref steeringForce, force))
                {
                    return steeringForce;
                }
            }

            if (OffsetPursuitEnabled)
            {
                force = OffsetPursuit(offset) * Params.GetWeight("offset_pursuit_weight");
                if (!accumulateForce(ref steeringForce, force))
                {
                    return steeringForce;
                }
            }

            if (FollowPathEnabled)
            {
                force = FollowPath() * Params.GetWeight("follow_path_weight");
                if (!accumulateForce(ref steeringForce, force))
                {
                    return steeringForce;
                }
            }

            if (RandomWalkEnabled)
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
            Utilities.checkNaN(force);
            Vector3 newAcceleration = force / mass;

            if (Params.drawVectors)
            {
                LineDrawer.DrawVectors(transform);
            }

            timeDelta = Time.deltaTime + Params.timeModifier;

            if (timeDelta > 0.0f)
            {
                smoothRate = Utilities.Clip(9.0f * timeDelta, 0.15f, 0.4f) / 2.0f;
                Utilities.BlendIntoAccumulator(smoothRate, newAcceleration, ref acceleration);
            }

            velocity += acceleration * timeDelta;

            float speed = velocity.magnitude;
            if (speed > maxSpeed)
            {
                velocity.Normalize();
                velocity *= maxSpeed;
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

            path.Draw();

            //Vector3 acceleration = Calculate() / mass;
            //velocity += acceleration * Time.deltaTime;
            //gameObject.transform.position += velocity * Time.deltaTime;

            //force = Vector3.zero;

            //if (velocity.magnitude > 0.01f)
            //{
            //    gameObject.transform.forward = Vector3.Normalize(velocity);
            //}

            //velocity *= 0.99f;
        }

        #endregion

        #region Behaviours

        Vector3 Seek(Vector3 targetPos)
        {
            Vector3 desiredVelocity;

            desiredVelocity = targetPos - transform.position;
            desiredVelocity.Normalize();
            desiredVelocity *= maxSpeed;
            if (Params.drawDebugLines)
            {
                LineDrawer.DrawTarget(targetPos, Color.red);
            }
            return (desiredVelocity - velocity);
        }

        Vector3 Evade()
        {
            float dist = (target.transform.position - transform.position).magnitude;
            float lookAhead = maxSpeed;

            Vector3 targetPos = target.transform.position + (lookAhead * target.GetComponent<SteeringBehaviours>().velocity);
            return Flee(targetPos);
        }

        Vector3 ObstacleAvoidance()
        {
            Vector3 force = Vector3.zero;
            makeFeelers();
            List<GameObject> tagged = new List<GameObject>();
            float minBoxLength = 20.0f;
            float boxLength = minBoxLength + ((velocity.magnitude / maxSpeed) * minBoxLength * 2.0f);

            if (float.IsNaN(boxLength))
            {
                System.Console.WriteLine("NAN");
            }
            // Matt Bucklands Obstacle avoidance
            // First tag obstacles in range
            GameObject[] obstacles = GameObject.FindGameObjectsWithTag("obstacle");
            if (obstacles.Length == 0)
            {
                return Vector3.zero;
            }
            foreach (GameObject obstacle in obstacles)
            {
                Vector3 toCentre = transform.position - obstacle.transform.position;
                float dist = toCentre.magnitude;
                if (dist < boxLength)
                {
                    tagged.Add(obstacle);
                }
            }

            float distToClosestIP = float.MaxValue;
            GameObject closestIntersectingObstacle = null;
            Vector3 localPosOfClosestObstacle = Vector3.zero;
            Vector3 intersection = Vector3.zero;

            foreach (GameObject o in tagged)
            {
                Vector3 localPos = transform.InverseTransformPoint(o.transform.position);

                // If the local position has a positive Z value then it must lay
                // behind the agent. (in which case it can be ignored)
                if (localPos.z >= 0)
                {
                    // If the distance from the x axis to the object's position is less
                    // than its radius + half the width of the detection box then there
                    // is a potential intersection.

                    float obstacleRadius = o.transform.localScale.x / 2;
                    float expandedRadius = myRadius + obstacleRadius;
                    if ((Math.Abs(localPos.y) < expandedRadius) && (Math.Abs(localPos.x) < expandedRadius))
                    {
                        // Now to do a ray/sphere intersection test. The center of the				
                        // Create a temp Entity to hold the sphere in local space
                        Sphere tempSphere = new Sphere(expandedRadius, localPos);

                        // Create a ray
                        BGE.Geom.Ray ray = new BGE.Geom.Ray();
                        ray.pos = new Vector3(0, 0, 0);
                        ray.look = Vector3.forward;

                        // Find the point of intersection
                        if (tempSphere.closestRayIntersects(ray, Vector3.zero, ref intersection) == false)
                        {
                            return Vector3.zero;
                        }

                        // Now see if its the closest, there may be other intersecting spheres
                        float dist = intersection.magnitude;
                        if (dist < distToClosestIP)
                        {
                            dist = distToClosestIP;
                            closestIntersectingObstacle = o;
                            localPosOfClosestObstacle = localPos;
                        }
                    }
                }
                if (closestIntersectingObstacle != null)
                {
                    // Now calculate the force
                    // Calculate Z Axis braking  force
                    float multiplier = 200 * (1.0f + (boxLength - localPosOfClosestObstacle.z) / boxLength);

                    //calculate the lateral force
                    float obstacleRadius = closestIntersectingObstacle.GetComponent<Renderer>().bounds.extents.magnitude;
                    float expandedRadius = myRadius + obstacleRadius;
                    force.x = (expandedRadius - Math.Abs(localPosOfClosestObstacle.x)) * multiplier;
                    force.y = (expandedRadius - -Math.Abs(localPosOfClosestObstacle.y)) * multiplier;

                    if (localPosOfClosestObstacle.x > 0)
                    {
                        force.x = -force.x;
                    }

                    if (localPosOfClosestObstacle.y > 0)
                    {
                        force.y = -force.y;
                    }

                    if (Params.drawDebugLines)
                    {
                        LineDrawer.DrawLine(transform.position, transform.position + transform.forward * boxLength, debugLineColour);                    
                    }
                    //apply a braking force proportional to the obstacle's distance from
                    //the vehicle.
                    const float brakingWeight = 40.0f;
                    force.z = (obstacleRadius -
                                       localPosOfClosestObstacle.z) *
                                       brakingWeight;

                    //finally, convert the steering vector from local to world space
                    force = transform.TransformPoint(force);
                }
            }


            return force;
        }

        Vector3 OffsetPursuit(Vector3 offset)
        {
            Vector3 target = Vector3.zero;
            target = leader.transform.TransformPoint(offset);
         
            float dist = (target - transform.position).magnitude;

            float lookAhead = (dist / maxSpeed);

            target = target + (lookAhead * leader.GetComponent<SteeringBehaviours>().velocity);

            Utilities.checkNaN(target);
            return Arrive(target);
        }

        Vector3 Pursue()
        {
            Vector3 toTarget = leader.transform.position - transform.position;
            float dist = toTarget.magnitude;
            float time = dist / maxSpeed;

            Vector3 targetPos = leader.transform.position + (time * leader.GetComponent<SteeringBehaviours>().velocity);
            if (Params.drawDebugLines)
            {
                LineDrawer.DrawLine(transform.position, targetPos, debugLineColour);
            }

            return Seek(targetPos);
        }

        Vector3 Flee(Vector3 targetPos)
        {
            float panicDistance = 100.0f;
            Vector3 desiredVelocity;
            desiredVelocity = transform.position - targetPos;
            if (desiredVelocity.magnitude > panicDistance)
            {
                return Vector3.zero;
            }
            desiredVelocity.Normalize();
            desiredVelocity *= maxSpeed;
            return (desiredVelocity - velocity);
        }

        Vector3 RandomWalk()
        {
            float dist = (transform.position - randomWalkTarget).magnitude;
            if (dist < 50)
            {
                randomWalkTarget.x = Utilities.RandomClamped() * Params.GetFloat("world_range");
                randomWalkTarget.y = Utilities.RandomClamped(0, Params.GetFloat("world_range") / 2.0f);
                randomWalkTarget.z = Utilities.RandomClamped() * Params.GetFloat("world_range");
            }
            return Seek(randomWalkTarget);
        }



        Vector3 Wander()
        {
            float jitterTimeSlice = Params.GetFloat("wander_jitter") * timeDelta;

            Vector3 toAdd = new Vector3(Utilities.RandomClamped(), Utilities.RandomClamped(), Utilities.RandomClamped()) * jitterTimeSlice;
            wanderTargetPos += toAdd;
            wanderTargetPos.Normalize();
            wanderTargetPos *= Params.GetFloat("wander_radius");

            Vector3 worldTarget = transform.TransformPoint(wanderTargetPos + (Vector3.forward * Params.GetFloat("wander_distance")));
            return (worldTarget - transform.position);
        }

        public Vector3 WallAvoidance()
        {
            makeFeelers();

            Plane worldPlane = new Plane(new Vector3(0, 1, 0), 0);
            Vector3 force = Vector3.zero;

            foreach (Vector3 feeler in Feelers)
            {
                if (!worldPlane.GetSide(feeler))
                {
                    float distance = Math.Abs(worldPlane.GetDistanceToPoint(feeler));
                    force += worldPlane.normal * distance;
                }
            }

            if (force.magnitude > 0.0)
            {
                DrawFeelers();
            }
            return force;
        }

        public void DrawFeelers()
        {
            if (Params.drawDebugLines)
            {
                foreach (Vector3 feeler in Feelers)
                {
                    LineDrawer.DrawLine(transform.position, feeler, debugLineColour);
                }
            }            
        }

        public Vector3 Arrive(Vector3 target)
        {
            Vector3 toTarget = target - transform.position;

            float slowingDistance = 8.0f;
            float distance = toTarget.magnitude;
            if (distance == 0.0f)
            {
                return Vector3.zero;
            }
            const float DecelerationTweaker = 10.3f;
            float ramped = maxSpeed * (distance / (slowingDistance * DecelerationTweaker));

            float clamped = Math.Min(ramped, maxSpeed);
            Vector3 desired = clamped * (toTarget / distance);
            if (Params.drawDebugLines)
            {
                LineDrawer.DrawTarget(target, Color.gray);
            }
            Utilities.checkNaN(desired);


            return desired - velocity;
        }

        private Vector3 FollowPath()
        {
            float epsilon = 5.0f;
            float dist = (transform.position - path.NextWaypoint()).magnitude;
            if (dist < epsilon)
            {
                path.AdvanceToNext();
            }
            if ((!path.Looped) && path.IsLast())
            {
                return Arrive(path.NextWaypoint());
            }
            else
            {
                return Seek(path.NextWaypoint());
            }
        }

        public Vector3 SphereConstrain(float radius)
        {
            float distance = transform.position.magnitude;
            Vector3 steeringForce = Vector3.zero;
            if (distance > radius)
            {
                steeringForce = Vector3.Normalize(transform.position) * (radius - distance);
            }
            return steeringForce;
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
            Vector3 steeringForce = Vector3.zero;
            for (int i = 0; i < tagged.Count; i++)
            {
                GameObject entity = tagged[i];
                if (entity != null)
                {
                    Vector3 toEntity = transform.position - entity.transform.position;
                    steeringForce += (Vector3.Normalize(toEntity) / toEntity.magnitude);
                }
            }

            return steeringForce;
        }

        public Vector3 Cohesion()
        {
            Vector3 steeringForce = Vector3.zero;
            Vector3 centreOfMass = Vector3.zero;
            int taggedCount = 0;
            foreach (GameObject entity in tagged)
            {
                if (entity != gameObject)
                {
                    centreOfMass += entity.transform.position;
                    taggedCount++;
                }
            }
            if (taggedCount > 0)
            {
                centreOfMass /= (float)taggedCount;

                if (centreOfMass.sqrMagnitude == 0)
                {
                    steeringForce = Vector3.zero;
                }
                else
                {
                    steeringForce = Vector3.Normalize(Seek(centreOfMass));
                }
            }
            Utilities.checkNaN(steeringForce);
            return steeringForce;
        }

        public Vector3 Alignment()
        {
            Vector3 steeringForce = Vector3.zero;
            int taggedCount = 0;
            foreach (GameObject entity in tagged)
            {
                if (entity != gameObject)
                {
                    steeringForce += entity.transform.forward;
                    taggedCount++;
                }
            }

            if (taggedCount > 0)
            {
                steeringForce /= (float)taggedCount;
                steeringForce -= transform.forward;
            }
            return steeringForce;

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
            turnOffAll();
            maxSpeed = Params.GetFloat("max_speed");
            calculationMethod = CalculationMethods.WeightedTruncatedRunningSumWithPrioritisation;
            target = null;
            leader = null;
        }
    }
}
