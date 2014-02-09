using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BGE
{
    public class Node : IComparable<Node>
    {
        public Vector3 pos;
        public float f, g, h;
        public Node parent;

        public int CompareTo(Node b)
        {
            if ((this.f < b.f))
            {
                return -1;
            }
            if (this.f == b.f)
            {
                return 0;
            }
            return 1;
        }
    }
}
