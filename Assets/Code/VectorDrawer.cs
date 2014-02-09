using UnityEngine;
using System.Collections;

namespace BGE
{
    public class VectorDrawer : MonoBehaviour
    {
        Vector3 position;
        Vector3 look;
        Vector3 right;
        Vector3 up;
        Quaternion orientation;

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

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            UpdateMe();
            Debug.DrawLine(position, position + look * 5.0f, Color.green);
            Debug.DrawLine(position, position + up * 5.00f, Color.red);
            Debug.DrawLine(position, position + right * 5.0f, Color.blue);
        }
    }
}