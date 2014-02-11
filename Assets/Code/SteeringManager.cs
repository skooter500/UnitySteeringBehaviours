using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using BGE.Scenarios;

namespace BGE
{
    

    public class SteeringManager : MonoBehaviour
    {
        List<Scenario> scenarios = new List<Scenario>();
        
        public Scenario currentScenario;
        StringBuilder message = new StringBuilder();
        
        public GameObject camFighter;

        public GameObject boidPrefab;
        public GameObject leaderPrefab;

        static SteeringManager instance;
        // Use this for initialization
        GUIStyle style = new GUIStyle();

        bool camFollowing = false;
        public bool showMessages = true;

        void Awake()
        {
            DontDestroyOnLoad(this);
        }

        void Start()
        {
            instance = this;
            Screen.showCursor = false;

            style.fontSize = 18;
            style.normal.textColor = Color.white;

            scenarios.Add(new SeekScenario());
            scenarios.Add(new ArriveScenario());
            scenarios.Add(new PursueScenario());
            scenarios.Add(new PathFollowingScenario());
            scenarios.Add(new ObstacleAvoidanceScenario());
            scenarios.Add(new FlockingScenario());
            scenarios.Add(new StateMachineScenario());
            scenarios.Add(new PathFindingScenario());
            currentScenario = scenarios[0];
            currentScenario.Start();

        }

        public static SteeringManager Instance()
        {
            return instance;
        }

        

        void OnGUI()
        {
            if (showMessages)
            {
                GUI.Label(new Rect(0, 0, Screen.width, Screen.height), "" + message, style);
            }
            if (Event.current.type == EventType.Repaint)
            {
                message.Length = 0;
            }

            if (Event.current.type == EventType.KeyDown)
            {
                if (Event.current.keyCode == KeyCode.F1)
                {
                    camFollowing = !camFollowing;
                }

                for (int i = 0; i < scenarios.Count; i++)
                {
                    if (Event.current.keyCode == KeyCode.Alpha0 + i)
                    {
                        currentScenario.TearDown();
                        currentScenario = scenarios[i];
                        currentScenario.Start();
                    }
                }

                if (Event.current.keyCode == KeyCode.F2)
                {
                    Params.Put(Params.TIME_MODIFIER_KEY, Params.GetFloat(Params.TIME_MODIFIER_KEY) + Time.deltaTime);
                }

                if (Event.current.keyCode == KeyCode.F3)
                {
                    Params.Put(Params.TIME_MODIFIER_KEY, Params.GetFloat(Params.TIME_MODIFIER_KEY) - Time.deltaTime);
                }

                if (Event.current.keyCode == KeyCode.F4)
                {
                    showMessages = ! showMessages;
                }

                if (Event.current.keyCode == KeyCode.Escape)
                {
                    Application.Quit();
                }
                
            }
        }

        public static void PrintMessage(string message)
        {
            Instance().message.Append(message + "\n");
        }

        public static void PrintFloat(string message, float f)
        {
            Instance().message.Append(message + ": " + f + "\n");
        }

        public static void PrintVector(string message, Vector3 v)
        {
            Instance().message.Append(message + ": (" + v.x + ", " + v.y + ", " + v.z + ")\n");
        }

        // Update is called once per frame
        void Update()
        {
            PrintMessage("Press F1 to toggle cam following");
            PrintMessage("Press F2 to increase timeDelta");
            PrintMessage("Press F3 to decrease timeDelta");
            PrintMessage("Press F4 to toggle messages");
            int fps = (int)(1.0f / Time.deltaTime);
            PrintFloat("FPS: ", fps);
            PrintMessage("Current scenario: " + currentScenario.Description());
            for (int i = 0; i < scenarios.Count; i++)
            {
                PrintMessage("Press " + i + " for " + scenarios[i].Description());
            }

            if (camFollowing)
            {
                GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");
                camera.transform.position = camFighter.transform.position;
                camera.transform.rotation = camFighter.transform.rotation;
            }
      
            currentScenario.Update();
        }
    }
}
