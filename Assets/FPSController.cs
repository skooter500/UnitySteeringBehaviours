using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class FPSController : MonoBehaviour {

	Vector3 position;
	Vector3 look;
    Vector3 right;
    Vector3 up;
    Quaternion orientation;
    float speed = 5.0f;
    float mouseX, mouseY;
    StringBuilder message = new StringBuilder();
    // Use this for initialization
	void Start () 
	{ 
		Screen.showCursor = false;
		Screen.lockCursor = true;
	}

    void OnGUI()
    {
        GUI.Label(new Rect(0, 0, Screen.width, Screen.height), "" + message);
        Debug.Log("" + message);
    }

    void AddMessage(string message)
    {
        this.message.Append(message + "\n");
    }

	void Yaw(float angle)
	{
		Quaternion rot = Quaternion.AngleAxis (angle, Vector3.up);
		orientation = rot * orientation;
	}

	void Pitch(float angle)
	{
		float invcosTheta1 = Vector3.Dot(look, Vector3.up);
		float threshold = 0.95f;
		if ((angle > 0 && invcosTheta1 < (-threshold)) || (angle < 0 && invcosTheta1 > (threshold)))
		{
			return;
		}

		// A pitch is a rotation around the right vector
		Quaternion rot = Quaternion.AngleAxis(angle, right);

		orientation = rot * orientation;
	}

	void UpdateMe()
	{
		position = gameObject.transform.position;
		orientation = gameObject.transform.rotation;
		look = gameObject.transform.forward;
		right = gameObject.transform.right;
        up = gameObject.transform.up;
	}

	void UpdateGameObject()
	{
		gameObject.transform.position = position;
		gameObject.transform.rotation = orientation;
	}
	// Update is called once per frame
	void Update () 
	{
		message.Length = 0;
		UpdateMe();        

		if (Input.GetKey(KeyCode.W)) 
		{
			position +=  gameObject.transform.forward * Time.deltaTime * speed;
		}

        if (Input.GetKey(KeyCode.S))
        {
            position -= gameObject.transform.forward * Time.deltaTime * speed;
        }

        if (Input.GetKey(KeyCode.A))
        {
            position -= gameObject.transform.right * Time.deltaTime * speed;
        }

        if (Input.GetKey(KeyCode.D))
        {
            position += gameObject.transform.right * Time.deltaTime * speed;
        }

        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");

        AddMessage("Mouse X" + mouseX);
        AddMessage("Mouse Y" + mouseY);

		float invcosTheta1 = Vector3.Dot(look, Vector3.up);
		float angle = Mathf.Acos (invcosTheta1);
		AddMessage("InvCostheta: " + invcosTheta1);
		AddMessage("Pitch angle: " + angle);

		Yaw(mouseX);
		Pitch(-mouseY);

		UpdateGameObject();
	}
}