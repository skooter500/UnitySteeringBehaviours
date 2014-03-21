using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BGE
{
    public class Cell
    {
        public Bounds bounds = new Bounds();
        public int number;
        public List<GameObject> contained = new List<GameObject>(500);
        public List<Cell> adjacent = new List<Cell>();

        public bool Contains(Vector3 pos)
        {
            return bounds.Contains(pos);
        }

        public bool Intersects(Bounds b)
        {
            if ((b.min.x > bounds.max.x) || (b.max.x < bounds.min.x)
                || (b.min.z > bounds.max.z) || (b.max.z < bounds.min.z))
            {
                return false;
            }
            else
            {
                return true;
            }

            /*Vector3 min = box.min;
            min.y = 0;
            box.min = min;

            Vector3 max = box.max;
            max.y = 0;
            box.max = max;
            return this.box.Intersects(box);
             * */
        }
    }
}
