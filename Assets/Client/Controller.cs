using UnityEngine;

namespace Client
{
    public class Controller : MonoBehaviour
    {
        public float rotSpeed = 150.0f;
        public float movSpeed = 3.0f;
        float y = 0, z = 0;
        void Update()
        {
            z += Input.GetAxis("Horizontal") * Time.deltaTime * rotSpeed;
            y += Input.GetAxis("Vertical") * Time.deltaTime * movSpeed;

        }
        void FixedUpdate()
        {
            transform.Rotate(0, 0, -z);
            transform.Translate(0, y, 0);
            y = 0;
            z = 0;
        }

    }
}
