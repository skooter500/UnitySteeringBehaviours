using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BGE
{
	class Lazer:MonoBehaviour
	{
        public void Update()
        {
            float speed = 5.0f;
            float width = Params.GetFloat("world_range");
            float height = Params.GetFloat("world_range");

            if ((transform.position.x < -(width / 2)) || (transform.position.x > width / 2) || (transform.position.z < -(height / 2)) || (transform.position.z > height / 2) || (transform.position.y < 0) || (transform.position.y > 100))
            {
                Destroy(gameObject);
            }
            transform.position += transform.forward * speed;
            Debug.DrawLine(transform.position, transform.position + transform.forward * 10.0f, Color.red);
        }
	}
}
