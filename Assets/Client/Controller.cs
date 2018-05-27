using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour {
    public float rotSpeed = 150.0f;
    public float movSpeed = 3.0f;
    void Update()
    {
        var x = Input.GetAxis("Horizontal") * Time.deltaTime * rotSpeed;
        var z = Input.GetAxis("Vertical") * Time.deltaTime * movSpeed;

        transform.Rotate(0, x, 0);
        transform.Translate(0, 0, z);
    }

}
