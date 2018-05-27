using System;
using System.Linq;
using System.Net;
using System.Text;
using UnityEngine;

namespace Server
{
    public class Player : MonoBehaviour
    {
        static int delay = 20;
        public IPAddress ip;
        public int port;
        public string id;
        public bool valid;
        Connection con;
        public Vector2 position;
        public Vector2 rotation;
        public GameObject me;
        public static Vector2 DataToVec2(byte[] data, int offset)
        {
            return new Vector2
            {
                x = BitConverter.ToSingle(data, 0 + offset),
                y = BitConverter.ToSingle(data, 4 + offset)
            };
        }
        public byte[] InfoToData()
        {
            return BitConverter.GetBytes(id.Length)
                .Concat(Encoding.ASCII.GetBytes(id))
                .Concat(BitConverter.GetBytes(position.x))
                .Concat(BitConverter.GetBytes(position.y))
                .Concat(BitConverter.GetBytes(rotation.x))
                .Concat(BitConverter.GetBytes(rotation.y)).ToArray();
        }
        public Player(IPAddress ip, int playerPort, int recievePort)
        {
            valid = true;
            this.ip = ip;
            port = playerPort;
            id = recievePort.ToString();
            this.position = new Vector2() { x = 0, y = 0 };
            con = new Connection(IPAddress.Any, recievePort, Handle, delay);
        }
        public void Send(byte[] data)
        {
            con.Send(data, ip, port);
        }
        public void Handle(byte[] data, IPAddress ip, int port)
        {
            if ( !ip.Equals(this.ip) )
            {
                Debug.Log("Error sending info to player");
                return;
            }
            Command cmd = (Command)BitConverter.ToInt32(data, 0);
            switch(cmd)
            {
                case Command.Join:
                    this.port = port;
                    break;
                case Command.Send:
                    position = DataToPos(data, 4);
                    rotation = DataToPos(data, 12);

                    me = GetComponent<MixedData>().CreatePlayer(position, rotation);
                    break;
                case Command.Quit:
                    valid = false;
                    break;
            }
        }
    }
}
