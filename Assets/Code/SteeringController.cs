using UnityEngine;
using System.Collections;

namespace BGE
{
    public class SteeringController : MonoBehaviour
    {
        public Vector3 force;
        public Vector3 velocity;

        public Vector3 targetPos;

        public float mass;

        public KeyCode forwardKey;
        public KeyCode backKey;
        public KeyCode leftKey;
        public KeyCode rightKey;



        public SteeringController()
        {
            force = Vector3.zero;
            velocity = Vector3.zero;
            mass = 10.0f;

            forwardKey = KeyCode.I;
            backKey = KeyCode.K;
            rightKey = KeyCode.L;
            leftKey = KeyCode.J;

        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            float newtons = 100.0f;
            if (Input.GetKey(forwardKey))
            {
                force += newtons * gameObject.transform.forward;
            }

            if (Input.GetKey(backKey))
            {
                force -= newtons * gameObject.transform.forward;
            }

            if (Input.GetKey(leftKey))
            {
                force -= newtons * gameObject.transform.right;
            }

            if (Input.GetKey(rightKey))
            {
                force += newtons * gameObject.transform.right;
            }

            Vector3 acceleration = force / mass;
            velocity += acceleration * Time.deltaTime;
            gameObject.transform.position += velocity * Time.deltaTime;

            force = Vector3.zero;
            if (velocity.magnitude > 0.01f)
            {
                gameObject.transform.forward = Vector3.Normalize(velocity);
            }
            velocity *= 0.99f;
        }
    }
}
