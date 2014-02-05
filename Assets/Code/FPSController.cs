using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class FPSController : MonoBehaviour {

    float speed = 5.0f;
    float mouseX, mouseY;
    // Use this for initialization
	void Start () 
	{ 
		Screen.showCursor = false;
		Screen.lockCursor = true;
	}

	void Yaw(float angle)
	{
		Quaternion rot = Quaternion.AngleAxis (angle, Vector3.up);
        transform.rotation = rot * transform.rotation;
	}

	void Pitch(float angle)
	{
		float invcosTheta1 = Vector3.Dot(transform.forward, Vector3.up);
		float threshold = 0.95f;
		if ((angle > 0 && invcosTheta1 < (-threshold)) || (angle < 0 && invcosTheta1 > (threshold)))
		{
			return;
		}

		// A pitch is a rotation around the right vector
		Quaternion rot = Quaternion.AngleAxis(angle, transform.right);

        transform.rotation = rot * transform.rotation;
	}

	// Update is called once per frame
	void Update () 
	{
        float speed = this.speed;
		
        if (Input.GetKey(KeyCode.LeftShift))
        {
            speed *= 10.0f;
        }
        
        if (Input.GetKey(KeyCode.W)) 
		{
			transform.position +=  gameObject.transform.forward * Time.deltaTime * speed;
		}

        if (Input.GetKey(KeyCode.S))
        {
            transform.position -= gameObject.transform.forward * Time.deltaTime * speed;
        }

        if (Input.GetKey(KeyCode.A))
        {
            transform.position -= gameObject.transform.right * Time.deltaTime * speed;
        }

        if (Input.GetKey(KeyCode.D))
        {
            transform.position += gameObject.transform.right * Time.deltaTime * speed;
        }

        GameManager.PrintVector("Cam pos: ", transform.position);
        GameManager.PrintVector("Cam forward: ", transform.position);

        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");

		Yaw(mouseX);
		Pitch(-mouseY);
	}
}