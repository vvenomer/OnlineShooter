using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Linq;
using UnityEngine;

namespace Server
{
    class Server : MonoBehaviour
    {
        public GameObject mainCamera;
        int port;
        List<Player> players;
        Connection joinConnection;
        MixedData data;
        void HandleJoining(byte[] data, IPAddress ip, int port)
        {
            this.port++;
            //is it right packet
            if ((Command)BitConverter.ToInt32(data, 0) != Command.Connect) return;
            //send mesege to console
            Debug.Log("Nowe połączenie od:" + ip.ToString() + ":" + port);
            //send back info where to send next packets
            byte[] message = BitConverter.GetBytes(this.port);
            joinConnection.Send(message, ip, port);
            //create new player
            Player newPlayer = new Player(ip, port, this.port);
            players.Add(newPlayer);
        }
        void SendInfo()
        {
            if (players.Count == 0) return;
            byte[] playersInfo = new byte[0];
            //get players info
            foreach (Player player in players)
            {
                if (!player.valid) //player has quited
                {
                    continue;
                }
                playersInfo = playersInfo.Concat(player.InfoToData()).ToArray();
            }
            //send it to players
            foreach (Player player in players)
            {
                player.Send(playersInfo);
            }
        }
        void Update()
        {

            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].valid && players[i].myController == null)
                {
                    players[i].me = this.data.CreatePlayer(Vector2.zero, 0);
                    players[i].myController = players[i].me.AddComponent<Controller>();
                }
                if (!players[i].valid) //player has quited
                {
                    Destroy(players[i].me);
                    players.Remove(players[i]);
                    i--;
                }
                //update player positions in scene
            }
        }

        void Start()
        {
            data = GetComponent<MixedData>();
            if(GetComponent<Client.Client>().enabled == false )
                Instantiate(mainCamera);
            players = new List<Player>();
            port = data.port;
            joinConnection = new Connection(IPAddress.Any, port, HandleJoining, data.sendToServerInterval);
            new ProcFunc(SendInfo, data.sendToClientInterval);
        }
    }
}
