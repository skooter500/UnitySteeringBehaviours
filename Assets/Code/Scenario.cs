using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public abstract class Scenario
{
    public GameObject leader = GameManager.Instance().leaderPrefab;
    public GameObject boid = GameManager.Instance().boidPrefab;
    
    
    public abstract string Description();
    public abstract void SetUp();

    public virtual void Update()
    {
        /*MouseState mouseState = Mouse.GetState();
        if (mouseState.LeftButton == ButtonState.Pressed)
        {
            Vector3 newTargetPos = XNAGame.Instance.Camera.Position + (XNAGame.Instance.Camera.Look * 100.0f);
            //newTargetPos.Y = 8;
            XNAGame.Instance.Leader.TargetPos = newTargetPos;

        }

        if (mouseState.RightButton == ButtonState.Pressed)
        {
            Vector3 newTargetPos = XNAGame.Instance.Camera.Position;
            XNAGame.Instance.Leader.TargetPos = newTargetPos;

        }
         */
    }
    public abstract void TearDown();
}