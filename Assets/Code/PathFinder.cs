using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using BGE.Geom;

namespace BGE
{
    public class PathFinder
    {
        float dist = 5.0f;
        public string message = "";
            
        public PathFinder()
        {
            obstacles = GameObject.FindGameObjectsWithTag("obstacle");
            foreach (GameObject o in obstacles)
            {
                float radius = o.renderer.bounds.extents.magnitude;
                radii.Add(radius);
            }           
        }

        public bool isThreeD = false;


        Dictionary<Vector3, Node> open = new Dictionary<Vector3, Node>(20000);
        //PriorityQueue<Node> openPQ = new PriorityQueue<Node>();
        //List<Node> openList = new List<Node>();

        Dictionary<Vector3, Node> closed = new Dictionary<Vector3, Node>(20000);
        GameObject[] obstacles;
        List<float> radii = new List<float>();
        
        Vector3 start, end;

        bool smooth = false;

        public bool Smooth
        {
            get { return smooth; }
            set { smooth = value; }
        }

        Vector3 RoundToNodeDist(Vector3 v)
        {
            Vector3 ret = new Vector3();
            ret.x = ((int)(v.x / dist)) * dist;
            ret.y = ((int)(v.y / dist)) * dist;
            ret.z = ((int)(v.z / dist)) * dist;
            return ret;
        }

        public Path FindPath(Vector3 start, Vector3 end)
        {
            long oldNow = DateTime.Now.Ticks;
            bool found = false;
            this.end = RoundToNodeDist(start);
            this.start = RoundToNodeDist(end);

            open.Clear();
            closed.Clear();

            Node first = new Node();
            first.f = first.g = first.h = 0.0f;
            first.pos = this.start;
            open[this.start] = first;
            //openList.Add(first);
            //openPQ.Enqueue(first);

            Node current = first;
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            int maxSize = 0;
            stopwatch.Start();
            bool timeout = false;
            while (open.Count > 0)
            {
                if (stopwatch.ElapsedMilliseconds > 5000)
                {
                    timeout = true;
                    break;
                }
                if (open.Count > maxSize)
                {
                    maxSize = open.Count;
                }
                //current = openPQ.Dequeue();
                //float min = current.f;
                
                // Get the top of the q
                float min = float.MaxValue;
                foreach (Node node in open.Values)
                {
                    if (node.f < min)
                    {
                        current = node;
                        min = node.f;
                    }
                }
                
                if (current.pos.Equals(this.end))
                {
                    found = true;
                    break;
                }
                addAdjacentNodes(current);
                open.Remove(current.pos);
                //openList.Remove(current);
                closed[current.pos] = current;
            }
            Path path = new Path();
            if (found)
            {
                while (!current.pos.Equals(this.start))
                {
                    path.Waypoints.Add(current.pos);
                    current = current.parent;
                }
                path.Waypoints.Add(current.pos);
                message = "A * took: " + stopwatch.ElapsedMilliseconds + " milliseconds. Open list: " + maxSize;

            }
            else
            {
                if (timeout)
                {
                    message = "A* timed out after 5 seconds. Open list: " + maxSize;
                }
                else
                {
                    message = "No path found. Open list: " + maxSize;
                }
                
            }
            if (smooth)
            {
                SmoothPath(path);
            }
            return path;
        }

        private void addAdjacentNodes(Node current)
        {
            // Forwards
            Vector3 pos;
            pos.x = current.pos.x;
            pos.y = current.pos.y;
            pos.z = current.pos.z + dist;
            AddIfValid(pos, current);

            // Forwards right
            pos.x = current.pos.x + dist;
            pos.y = current.pos.y;
            pos.z = current.pos.z + dist;
            AddIfValid(pos, current);

            // Right
            pos.x = current.pos.x + dist;
            pos.y = current.pos.y;
            pos.z = current.pos.z;
            AddIfValid(pos, current);

            // Backwards Right
            pos.x = current.pos.x + dist;
            pos.y = current.pos.y;
            pos.z = current.pos.z - dist;
            AddIfValid(pos, current);

            // Backwards
            pos.x = current.pos.x;
            pos.y = current.pos.y;
            pos.z = current.pos.z - dist;
            AddIfValid(pos, current);

            // Backwards Left
            pos.x = current.pos.x - dist;
            pos.y = current.pos.y;
            pos.z = current.pos.z - dist;
            AddIfValid(pos, current);

            // Left
            pos.x = current.pos.x - dist;
            pos.y = current.pos.y;
            pos.z = current.pos.z;
            AddIfValid(pos, current);

            // Forwards Left
            pos.x = current.pos.x - dist;
            pos.y = current.pos.y;
            pos.z = current.pos.z + dist;
            AddIfValid(pos, current);

            if (isThreeD)
            {
                // Above in front row
                pos.x = current.pos.x - dist;
                pos.y = current.pos.y + dist;
                pos.z = current.pos.z - dist;
                AddIfValid(pos, current);

                pos.x = current.pos.x;
                pos.y = current.pos.y + dist;
                pos.z = current.pos.z - dist;
                AddIfValid(pos, current);

                pos.x = current.pos.x + dist;
                pos.y = current.pos.y + dist;
                pos.z = current.pos.z - dist;
                AddIfValid(pos, current);

                
                // Above middle row
                pos.x = current.pos.x - dist;
                pos.y = current.pos.y + dist;
                pos.z = current.pos.z;
                AddIfValid(pos, current);

                pos.x = current.pos.x;
                pos.y = current.pos.y + dist;
                pos.z = current.pos.z;
                AddIfValid(pos, current);

                pos.x = current.pos.x + dist;
                pos.y = current.pos.y + dist;
                pos.z = current.pos.z;
                AddIfValid(pos, current);

                // Above back row
                pos.x = current.pos.x - dist;
                pos.y = current.pos.y + dist;
                pos.z = current.pos.z + dist;
                AddIfValid(pos, current);

                pos.x = current.pos.x;
                pos.y = current.pos.y + dist;
                pos.z = current.pos.z + dist;
                AddIfValid(pos, current);

                pos.x = current.pos.x + dist;
                pos.y = current.pos.y + dist;
                pos.z = current.pos.z + dist;
                AddIfValid(pos, current);

                // Below in front row
                pos.x = current.pos.x - dist;
                pos.y = current.pos.y - dist;
                pos.z = current.pos.z - dist;
                AddIfValid(pos, current);

                pos.x = current.pos.x;
                pos.y = current.pos.y - dist;
                pos.z = current.pos.z - dist;
                AddIfValid(pos, current);

                pos.x = current.pos.x + dist;
                pos.y = current.pos.y - dist;
                pos.z = current.pos.z - dist;
                AddIfValid(pos, current);

                // Below middle row
                pos.x = current.pos.x - dist;
                pos.y = current.pos.y - dist;
                pos.z = current.pos.z;
                AddIfValid(pos, current);

                pos.x = current.pos.x;
                pos.y = current.pos.y - dist;
                pos.z = current.pos.z;
                AddIfValid(pos, current);

                pos.x = current.pos.x + dist;
                pos.y = current.pos.y - dist;
                pos.z = current.pos.z;
                AddIfValid(pos, current);

                // Below back row
                pos.x = current.pos.x - dist;
                pos.y = current.pos.y - dist;
                pos.z = current.pos.z + dist;
                AddIfValid(pos, current);

                pos.x = current.pos.x;
                pos.y = current.pos.y - dist;
                pos.z = current.pos.z + dist;
                AddIfValid(pos, current);

                pos.x = current.pos.x + dist;
                pos.y = current.pos.y - dist;
                pos.z = current.pos.z + dist;
                AddIfValid(pos, current);
                 
            }

        }

        private bool IsNavigable(Vector3 pos)
        {
            for (int i = 0; i < obstacles.Count(); i ++)
            {
                GameObject o = obstacles[i];
                if (Vector3.Distance(o.transform.position, pos) < radii[i])
                {
                    return false;
                }
            }
            return true;
        }

        private void AddIfValid(Vector3 pos, Node parent)
        {

            if ((IsNavigable(pos)))
            {
                if (!closed.ContainsKey(pos))
                {
                    if (!open.ContainsKey(pos))
                    {
                        Node node = new Node();
                        node.pos = pos;
                        node.g = parent.g + cost(node.pos, parent.pos);
                        node.h = heuristic(pos, end);
                        node.f = node.g + node.h;
                        node.parent = parent;
                        //openPQ.Enqueue(node);
                        open[pos] = node;
                        //openList.Add(node);
                    }
                    else
                    {
                        // Edge relaxation?
                        Node node = open[pos];
                        float g = parent.g + cost(node.pos, parent.pos);
                        if (g < node.g)
                        {
                            node.g = g;
                            node.f = node.g + node.h;
                            node.parent = parent;
                        }
                    }
                }
            }
        }

        public void SmoothPath(Path path)
        {
            List<Vector3> wayPoints = path.Waypoints;

            if (wayPoints.Count < 3)
            {
                return;
            }

            int current;
            int middle;
            int last;

            current = 0;
            middle = current + 1;
            last = current + 2;

            while (last != wayPoints.Count)
            {

                Vector3 point0, point2;

                point0 = wayPoints[current];
                point2 = wayPoints[last];
                point0.y = 0;
                point2.y = 0;
                if ((RayTrace(point0, point2)))
                {
                    current++;
                    middle++;
                    last++;

                }
                else
                {
                    wayPoints.RemoveAt(middle);
                }
            }
        }

        private float heuristic(Vector3 v1, Vector3 v2)
        {
            return 10.0f * (Math.Abs(v2.x - v1.x) + Math.Abs(v2.y - v1.y) + Math.Abs(v2.z - v1.z));
        }

        private float cost(Vector3 v1, Vector3 v2)
        {
            int dist = (int)Math.Abs(v2.x - v1.x) + (int)Math.Abs(v2.y - v1.y) + (int)Math.Abs(v2.z - v1.z);
            return (dist == 1) ? 10 : 14;
        }

        private bool RayTrace(Vector3 point0, Vector3 point1)
        {
            foreach (GameObject o in obstacles)
            {
                float radius = o.renderer.bounds.extents.magnitude;
                Sphere sphere = new Sphere(radius, o.transform.position);
                BGE.Geom.Ray ray = new BGE.Geom.Ray();
                ray.look = point1 - point0;
                ray.look.Normalize();
                ray.pos = point0;
                Vector3 intersectionPoint = new Vector3();
                if (sphere.closestRayIntersects(ray, point0, ref intersectionPoint))
                {
                    float dist = (intersectionPoint - point0).magnitude;
                    float rayLength = (point1 - point0).magnitude;
                    if (dist < rayLength)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
