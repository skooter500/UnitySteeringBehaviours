using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

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

	
	// Update is called once per frame
	void Update () {
	
	}
}
