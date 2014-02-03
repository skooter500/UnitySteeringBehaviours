using UnityEngine;
using System.Collections;

public class SteeringBehaviours : MonoBehaviour {

    public Vector3 force;
    public Vector3 velocity;

    public Vector3 targetPos;

    public float mass;

    public float maxSpeed = 20;
    public float maxForce = 10;

    Vector3 Seek(Vector3 targetPos)
    {
        Vector3 desiredVelocity;

        desiredVelocity = targetPos - transform.position;
        desiredVelocity.Normalize();
        desiredVelocity *= maxSpeed;

        return (desiredVelocity - velocity);
    }

	// Use this for initialization
	void Start () {
	
	}
	
	public SteeringBehaviours()
    {
        force = Vector3.zero;
        velocity = Vector3.zero;
        mass = 10.0f;

        GameObject sphere = GameObject.FindGameObjectWithTag("sphere");
        targetPos = sphere.transform.position;    
    }

    Vector3 Calculate()
    {
        return Seek(targetPos);
    }

    void Update()
    {       
        Vector3 acceleration = Calculate() / mass;
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
