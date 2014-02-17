using UnityEngine;
using System.Collections;

public class ForceController : MonoBehaviour {
    public Vector3 gravity = new Vector3(0, -9.8f, 0);
    public Vector3 velocity = Vector3.zero;
    public Vector3 forceAcc = Vector3.zero;
    public float mass;
    public Vector3 target = new Vector3(20, 5, 20);
    float maxSpeed = 10.0f;

    public GameObject enemy;

    Vector3 Flee(GameObject enemy)
    {
        float fleeDist = 5.0f;
        Vector3 desired = enemy.transform.position - transform.position;

        if (desired.magnitude < fleeDist)
        {
            desired.Normalize();
            desired *= maxSpeed;
            return velocity - desired;
        }
        else
        {
            return Vector3.zero;
        }
    }

    Vector3 Seek(Vector3 target)
    {
        Vector3 desired = target - transform.position;
        desired.Normalize();
        desired *= maxSpeed;

        return desired - velocity;
    }

	// Use this for initialization
	void Start () {
        mass = 1.0f;
	}

	// Update is called once per frame
	void Update () {
        float forceAmount = 10;

        if (Input.GetKey(KeyCode.UpArrow))
        {
            forceAcc += transform.forward * forceAmount;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            forceAcc -= transform.forward * forceAmount;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            forceAcc -= transform.right * forceAmount;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            forceAcc += transform.right * forceAmount;
        }

        forceAcc += Flee(enemy);

        Vector3 accel = forceAcc / mass;
        //accel += gravity;
        velocity = velocity + accel * Time.deltaTime;
        transform.position = transform.position + velocity * Time.deltaTime;
        forceAcc = Vector3.zero;
        if (velocity.magnitude > float.Epsilon)
        {
            transform.forward = Vector3.Normalize(velocity) ;
        }
        velocity *= 0.99f; // Damping
	}
}
