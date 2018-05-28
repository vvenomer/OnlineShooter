using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    public class Client : MonoBehaviour
    {

        public GameObject mainCamera;

        public float rotSpeed = 150.0f;
        public float movSpeed = 3.0f;

        public string ip = "127.0.0.1";

        public Vector2 spawnPos = Vector2.zero;
        public float spawnAngle = 0;

        void Start()
        {
            var data = GetComponent<MixedData>();
            var playerMade = Instantiate(data.player);
            Instantiate(mainCamera, playerMade.transform);

            var controller = playerMade.AddComponent<Controller>();
            controller.rotSpeed = rotSpeed;
            controller.movSpeed = movSpeed;

            var sender = playerMade.AddComponent<SendPos>();
            sender.ip = ip;
            sender.port = data.port;
            sender.recieveInterval = data.sendToClientInterval;
            sender.playerPrefab = data.player;
            sender.sendInterval = data.sendToServerInterval;
        }
    }
}