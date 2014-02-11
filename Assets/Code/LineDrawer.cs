using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

// Must be attached to the camera so that OnPostRender gets called
namespace BGE
{
    public class LineDrawer : MonoBehaviour
    {

        struct Line
        {
            public Vector3 start;
            public Vector3 end;
            public Color color;
            public Line(Vector3 start, Vector3 end, Color color)
            {
                this.start = start;
                this.end = end;
                this.color = color;
            }
        }

        static LineDrawer instance;

        public static LineDrawer Instance()
        {
            return instance;
        }

        List<Line> lines = new List<Line>();

        Material lineMaterial;

        // Use this for initialization
        void Start()
        {
            instance = this;
        }

        // Update is called once per frame
        void Update()
        {

        }

        void Awake()
        {
            DontDestroyOnLoad(this);
        }

        public static void DrawLine(Vector3 start, Vector3 end, Color colour)
        {
            Instance().lines.Add(new Line(start, end, colour));
        }

        void CreateLineMaterial()
        {
            if (!lineMaterial)
            {
                lineMaterial = new Material("Shader \"Lines/Colored Blended\" {" +
                    "SubShader { Pass { " +
                    "    Blend SrcAlpha OneMinusSrcAlpha " +
                    "    ZWrite Off Cull Off Fog { Mode Off } " +
                    "    BindChannels {" +
                    "      Bind \"vertex\", vertex Bind \"color\", color }" +
                    "} } }");
                lineMaterial.hideFlags = HideFlags.HideAndDontSave;
                lineMaterial.shader.hideFlags = HideFlags.HideAndDontSave;
            }
        }

        void OnPostRender()
        {
            CreateLineMaterial();
            // set the current material
            lineMaterial.SetPass(0);
            GL.Begin(GL.LINES);
            foreach (Line line in lines)
            {
                GL.Color(line.color);
                GL.Vertex3(line.start.x, line.start.y, line.start.z);
                GL.Vertex3(line.end.x, line.end.y, line.end.z);

            }
            GL.End();
            lines.Clear();
        }
    }
}
