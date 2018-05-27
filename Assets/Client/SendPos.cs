using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class SendPos : MonoBehaviour
{
    public string ip = "127.0.0.1";
    public int port = 1024;
    public float sendInterval = 20;
    public int recieveInterval = 25;
    public GameObject playerPrefab;
    float currTime=0;
    bool connected = false;
    Connection serverConnection;
    string newName = null;
    class Player
    {
        public string id;
        public float x;
        public float y;
        public float z;
    };
    List<Player> players = new List<Player>();
    bool hasListChanged = false;
    void Start () {
        //get login ...
        Connect();
    }
    void Connect()
    {
        serverConnection = new Connection(IPAddress.Parse(ip), port, HandleResponse, recieveInterval);

        byte[] message = BitConverter.GetBytes((int)Command.Join);
        //byte[] usr = Encoding.ASCII.GetBytes(userName);
        //byte[] all = message.Concat(usr).ToArray();
        serverConnection.Send(message);
    }
    void HandleResponse(byte[] data, IPAddress ip, int port)
    {
        if (!connected)
        {
            /*serverConnection.Port =*/ this.port = BitConverter.ToInt32(data, 0);
            serverConnection = new Connection(IPAddress.Parse(this.ip), this.port, HandleResponse, recieveInterval);
            byte[] message = BitConverter.GetBytes((int)Command.Join);
            serverConnection.Send(message);
            newName = this.port.ToString();
            connected = true;
        }
        else
        {
            int i = 0;
            while(data.Length > i)
            {
                int idSize = BitConverter.ToInt32(data, i);
                i += 4;
                string id = Encoding.ASCII.GetString(data, i, idSize );
                i += idSize;
                float X = BitConverter.ToSingle(data, i);
                i += 4;
                float Y = BitConverter.ToSingle(data, i);
                i += 4;
                float Z = BitConverter.ToSingle(data, i);
                i += 4;
                Player player = players.Find(x => x.id == id);
                if (player == null)
                    players.Add(new Player
                        {
                            id = id,
                            x = X,
                            y = Y,
                            z = Z
                        });
                else
                {
                    player.x = X;
                    player.y = Y;
                    player.z = Z;
                }
            }
            hasListChanged = true;
        }
        return;
    }
    void CreatePlayer(Player player)
    {
        GameObject gameObject = GameObject.Find(player.id);
        if(gameObject!=null)
        {
            if(gameObject.name != this.name)
                gameObject.transform.position = new Vector3(player.x, player.y, player.z);
        }
        else
        {
            GameObject instance = Instantiate(playerPrefab);
            instance.name = player.id;
            instance.transform.position = new Vector3(player.x, player.y, player.z);
        }
    }
    void Update ()
    {
        if (connected)
        {
            if(newName!=null)
            {
                this.name = newName;
                newName = null;
            }
            if(hasListChanged)
            {
                foreach(Player player in players)
                CreatePlayer(player);
                hasListChanged = false;
            }
            currTime += Time.deltaTime;
            if (currTime >= sendInterval / 1000)
            {
                byte[] msg = BitConverter.GetBytes((int)Command.Send).
                    Concat(BitConverter.GetBytes(this.transform.position.x)).
                    Concat(BitConverter.GetBytes(this.transform.position.y)).
                    Concat(BitConverter.GetBytes(this.transform.position.z)).ToArray();

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
