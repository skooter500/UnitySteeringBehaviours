using UnityEngine;
using System.Collections;

public class Perception : MonoBehaviour {
    GameObject otherShip;

	// Use this for initialization
	void Start () {
        otherShip = GameObject.FindGameObjectWithTag("ferdelance");
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 toOtherShip = otherShip.transform.position - gameObject.transform.position;
        string message;
        if (toOtherShip.magnitude > 10.0f)
        {
            message = "Out of range";            
        }
        else
        {
            message = "In range";
        }
        GameManager.PrintMessage(message);
        Debug.Log("" + message);

        toOtherShip.Normalize();

        float dot = Vector3.Dot(toOtherShip, transform.forward);
        if (dot < 0)
        {
            GameManager.PrintMessage("Behind");
        }
        else
        {
            GameManager.PrintMessage("In front");
        }

        float fov = Mathf.Deg2Rad * (45.0f / 2.0f);
        float angle = Mathf.Acos(dot);
        if (angle < fov)
        {
            GameManager.PrintMessage("Inside FOV");
        }
        else
        {
            GameManager.PrintMessage("Outside FOV");
        }
	}
}
