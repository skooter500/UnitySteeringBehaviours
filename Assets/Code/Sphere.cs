using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BGE
{
    class Sphere
    {
        public float radius;
        public Vector3 Position;

        public Sphere(float radius, Vector3 position)
        {
            this.radius = radius;
            this.Position = position;
        }
        public bool closestRayIntersects(Ray ray, Vector3 point, ref Vector3 intersection)
        {
            // Calculate p0-c call it v

            Vector3 v = ray.pos - Position;
            Vector3 p0 = Vector3.zero, p1 = Vector3.zero;

            // Now calculate a, b and c
            float a, b, c;

            /*
             *  a = u.u
                b = 2u(p0 – pc)
                c = (p0 – c).(p0 – c) - r2
            */
            a = Vector3.Dot(ray.look, ray.look);
            b = 2.0f * Vector3.Dot(v, ray.look);
            c = Vector3.Dot(v, v) - (radius * radius);

            // Calculate the discriminant
            float discriminant = (b * b) - (4.0f * a * c);

            // Test for imaginary number
            // If discriminant > 0, calculate values for t0 and t1
            // Substitute into the ray equation and calculate the 2 intersection points
            // Push the interesctions into the vector intersections
            if (discriminant >= 0.0f)
            {

                discriminant = (float)Math.Sqrt(discriminant);

                float t0 = (-b + discriminant) / (2.0f * a);
                float t1 = (-b - discriminant) / (2.0f * a);

                p0 = ray.pos + (ray.look * t0);
                p1 = ray.pos + (ray.look * t1);

                if ((point - p0).magnitude < (point - p1).magnitude)
                {
                    intersection = p0;
                }
                else
                {
                    intersection = p1;
                }
                return true;
            }
            return false;
        }
    }
}

