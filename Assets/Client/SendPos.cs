using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using UnityEngine;

namespace Client
{
    public class SendPos : MonoBehaviour
    {
        class Player
        {
            public string id;
            public Vector2 pos;
            public float rot;
            public GameObject me;
        };
        public string ip;
        public int port;
        public float sendInterval;
        public int recieveInterval;
        public GameObject playerPrefab;

        float currTime = 0;
        bool connected = false;
        Connection serverConnection;
        string newName = null;
        List<Player> players = new List<Player>();
        bool hasListChanged = false;
        void Start()
        {
            //get login ...
            Connect();
        }
        void Connect()
        {
            serverConnection = new Connection(IPAddress.Parse(ip), port, HandleResponse, recieveInterval);

            byte[] message = BitConverter.GetBytes((int)Command.Connect);
            //byte[] usr = Encoding.ASCII.GetBytes(userName);
            //byte[] all = message.Concat(usr).ToArray();
            serverConnection.Send(message);
        }
        void HandleResponse(byte[] data, IPAddress ip, int port)
        {
            if (!connected)
            {
                this.port = BitConverter.ToInt32(data, 0);
                //delete old connection / somehow change it?
                serverConnection = new Connection(IPAddress.Parse(this.ip), this.port, HandleResponse, recieveInterval);
                byte[] message = BitConverter.GetBytes((int)Command.Connect);
                serverConnection.Send(message);
                newName = this.port.ToString();
                connected = true;
            }
            else
            {
                int i = 0;
                while (data.Length > i)
                {
                    int idSize = BitConverter.ToInt32(data, i);
                    i += 4;
                    string id = Encoding.ASCII.GetString(data, i, idSize);
                    i += idSize;
                    float pX = BitConverter.ToSingle(data, i);
                    i += 4;
                    float pY = BitConverter.ToSingle(data, i);
                    i += 4;
                    float rY = BitConverter.ToSingle(data, i);
                    i += 4;
                    Player player = players.Find(x => x.id == id);
                    if (player == null)
                        players.Add(new Player
                        {
                            id = id,
                            pos = new Vector2(pX, pY),
                            rot = rY
                        });
                    else
                    {
                        player.pos.x = pX;
                        player.pos.y = pY;
                        player.rot = rY;
                    }
                }
                hasListChanged = true;
            }
            return;
        }
        void UpdatePosition(Player player)
        {
            player.me.transform.position = player.pos;
            player.me.transform.rotation = Quaternion.Euler(0, 0, player.rot);
        }
        void CreatePlayer(Player player)
        {
            if (player.me != null)
            {
                if (player.id != this.name)
                {
                    UpdatePosition(player);
                }
            }
            else
            {
                if (player.id != this.name)
                    player.me = Instantiate(playerPrefab);
                else player.me = this.gameObject;
                player.me.name = player.id;
                UpdatePosition(player);
            }
        }
        void Update()
        {
            if (connected)
            {
                if (newName != null)
                {
                    this.name = newName;
                    newName = null;
                }
                if (hasListChanged)
                {
                    foreach (Player player in players)
                        CreatePlayer(player);
                    hasListChanged = false;
                }
                currTime += Time.deltaTime;
                if (currTime >= sendInterval / 1000)
                {
                    byte[] msg = BitConverter.GetBytes((int)Command.Send).
                        Concat(BitConverter.GetBytes(this.transform.position.x)).
                        Concat(BitConverter.GetBytes(this.transform.position.y)).
                        Concat(BitConverter.GetBytes(this.transform.rotation.eulerAngles.z)).ToArray();

                    serverConnection.Send(msg);
                    currTime = 0;
                }
            }
        }
        private void OnApplicationQuit()
        {
            if (connected)
            {
                byte[] cmd = BitConverter.GetBytes((int)Command.Quit);
                serverConnection.Send(cmd);
            }
        }
    }
}