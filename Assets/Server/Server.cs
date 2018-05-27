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
        int port = 1024;
        List<Player> players;
        Connection joinConnection;
        void HandleJoining(byte[] data, IPAddress ip, int port)
        {
            this.port++;
            //is it right packet
            if ((Command)BitConverter.ToInt32(data, 0) != Command.Join) return;
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
            for (int i = 0; i < players.Count; i++)
            {
                if (!players[i].valid) //player has quited
                {
                    players.Remove(players[i]);
                    i--;
                    continue;
                }
                playersInfo = playersInfo.Concat(players[i].InfoToData()).ToArray();
            }
            //send it to players
            foreach (Player player in players)
            {
                player.Send(playersInfo);
            }
        }
        void Update()
        {
            //update player positions in scene
        }
        void Start()
        {
            var data = GetComponent<MixedData>();
            Server prog = new Server
            {
                players = new List<Player>(),
                port = data.port
            };
            prog.joinConnection = new Connection(IPAddress.Any, port, prog.HandleJoining, data.sendToServerInterval);
            new ProcFunc(prog.SendInfo, data.sendToClientInterval);
            while (true)
            {
                prog.Update();
                Thread.Sleep(200);
            }
        }
    }
}
