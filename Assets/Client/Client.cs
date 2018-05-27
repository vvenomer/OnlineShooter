using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Client : MonoBehaviour {

    public GameObject player;
    public GameObject camera;

    public float rotSpeed = 150.0f;
    public float movSpeed = 3.0f;

    public string ip = "127.0.0.1";
    public int port = 1024;
    
	void Start () {
        var playerMade = Instantiate(player);
        Instantiate(camera, playerMade.transform);

        var controller = playerMade.AddComponent<Controller>();
        controller.rotSpeed = rotSpeed;
        controller.movSpeed = movSpeed;

        var sender = playerMade.AddComponent<SendPos>();
        
	}
}
