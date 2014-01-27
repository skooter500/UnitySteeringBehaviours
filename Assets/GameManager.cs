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
        string msg = new string(message.ToString().ToCharArray());
        GUI.Label(new Rect(0, 0, Screen.width, Screen.height), msg);
        Debug.Log("On GUI called: " + msg);
        //message.Length = 0;
    }

    public void AddMessage(string message)
    {
        this.message.Append(message + "\n");
    }

	
	// Update is called once per frame
	void Update () {
        Debug.Log("Update called");
	}
}
