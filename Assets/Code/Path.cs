using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class Path
{
    private List<Vector3> waypoints = new List<Vector3>();
    private int next = 0;
    public bool draw;

    public int Next
    {
        get { return next; }
        set { next = value; }
    }

    public List<Vector3> Waypoints
    {
        get { return waypoints; }
        set { waypoints = value; }
    }

    private bool looped;


    public bool Looped
    {
        get { return looped; }
        set { looped = value; }
    }

    public void Draw()
    {
        Debug.Log("In Path draw" + draw + " " + waypoints.Count());
        if (draw)
        {
            for (int i = 1; i < waypoints.Count(); i++)
            {
                Debug.DrawLine(waypoints[i - 1], waypoints[i], Color.cyan);
            }
            if (looped)
            {
                Debug.DrawLine(waypoints[0], waypoints[waypoints.Count() - 1], Color.cyan);
            }
        }
    }

    public Vector3 NextWaypoint()
    {
        return waypoints[next];
    }

    public bool IsLast()
    {
        return (next == waypoints.Count() - 1);
    }

    public void AdvanceToNext()
    {
        if (looped)
        {                
            next = (next + 1) % waypoints.Count();
        }
        else
        {
            if (next != waypoints.Count() - 1)
            {
                next = next + 1;
            }
        }
    }
}
