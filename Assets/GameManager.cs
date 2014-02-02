using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class GameManager : MonoBehaviour {

    StringBuilder message = new StringBuilder();

    static GameManager instance;
	// Use this for initialization

	void Awake () {
		DontDestroyOnLoad(this);
	}

	void Start () {
        instance = this;
        Screen.showCursor = false;
	}

    public static GameManager Instance()
    {
        return instance;
    }

    void OnGUI()
    {
        GUI.Label(new Rect(0, 0, Screen.width, Screen.height), "" + message);
        if (Event.current.type == EventType.Repaint)
        {
            message.Length = 0;
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
	void Update () {

	}
}
