using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace BGE
{
    public class FPSController : MonoBehaviour
    {

        float speed = 10.0f;
        
        // Use this for initialization
        void Start()
        {
            Screen.showCursor = false;
            Screen.lockCursor = true;
        }

        void Yaw(float angle)
        {
            Quaternion rot = Quaternion.AngleAxis(angle, Vector3.up);
            transform.rotation = rot * transform.rotation;
        }

        void Roll(float angle)
        {
            Quaternion rot = Quaternion.AngleAxis(angle, Vector3.forward);
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

        void Walk(float units)
        {
            if (Params.riftEnabled)
            {
                GameObject riftCamera = GameObject.FindGameObjectWithTag("ovrcamera");
                Camera camera = riftCamera.GetComponentInChildren<Camera>();
                transform.position += camera.transform.forward * units;
            }
            else
            {
                transform.position += transform.forward * units;
            }
        }

        void Strafe(float units)
        {
            if (Params.riftEnabled)
            {
                GameObject riftCamera = GameObject.FindGameObjectWithTag("ovrcamera");
                Camera camera = riftCamera.GetComponentInChildren<Camera>();
                transform.position += camera.transform.right * units;
            }
            else
            {
                transform.position += transform.right * units;
            }
        }

        // Update is called once per frame
        void Update()
        {
            float mouseX, mouseY;
            float speed = this.speed;

            if (Input.GetKey(KeyCode.LeftShift))
            {
                speed *= 10.0f;
            }

            if (Input.GetKey(KeyCode.W))
            {
                Walk(Time.deltaTime * speed);
            }

            if (Input.GetKey(KeyCode.S))
            {
                Walk(- Time.deltaTime * speed);
            }

            if (Input.GetKey(KeyCode.A))
            {
                Strafe(- Time.deltaTime * speed);
            }

            if (Input.GetKey(KeyCode.D))
            {
                Strafe(Time.deltaTime * speed);
            }
            if (Input.GetKey(KeyCode.Q))
            {
                Roll(- Time.deltaTime * speed);
            }
            if (Input.GetKey(KeyCode.E))
            {
                Roll(Time.deltaTime * speed);
            }

            

            mouseX = Input.GetAxis("Mouse X");
            mouseY = Input.GetAxis("Mouse Y");

            GameObject ovrplayer = GameObject.FindGameObjectWithTag("ovrcamera");
            if (ovrplayer != null)
            {
                ovrplayer.transform.position = transform.position;
            }
            
            Yaw(mouseX);
            float contYaw = Input.GetAxis("Yaw Axis");
            float contPitch = Input.GetAxis("Pitch Axis");
            Yaw(contYaw);

            // If in Rift mode, dont pitch
            if (!Params.riftEnabled)
            {
                Pitch(-mouseY);
                Pitch(contPitch);
            }

            float contWalk = Input.GetAxis("Walk Axis");
            float contStrafe = Input.GetAxis("Strafe Axis");
            Walk(-contWalk * speed * Time.deltaTime);
            Strafe(contStrafe * speed * Time.deltaTime);
            
            SteeringManager.PrintVector("Cam pos: ", transform.position);
            SteeringManager.PrintVector("Cam forward: ", transform.forward);
        }
    }
}