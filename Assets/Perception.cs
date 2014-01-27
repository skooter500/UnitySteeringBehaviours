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
        if (toOtherShip.magnitude > 20.0f)
        {
            message = "Out of range";            
        }
        else
        {
            message = "In range";
        }
        GameManager.Instance().AddMessage("In range");
        Debug.Log("" + message);

        toOtherShip.Normalize();

        float dot = Vector3.Dot(toOtherShip, transform.forward);
        if (dot < 0)
        {
            GameManager.Instance().AddMessage("Behind");
        }
        else
        {
            GameManager.Instance().AddMessage("In front");
        }

	}
}
