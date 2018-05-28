using System;
using System.Linq;
using System.Net;
using System.Text;
using UnityEngine;

namespace Server
{
    public class Player
    {
        static int delay = 20;
        public IPAddress ip;
        public int port;
        public bool valid;

        public string id;
        public Vector2 position;
        public float rotation;
        public Controller myController;
        public GameObject me;

        Connection con;
        public static Vector2 DataToVec2(byte[] data, int offset)
        {
            return new Vector2(BitConverter.ToSingle(data, 0 + offset), BitConverter.ToSingle(data, 4 + offset));
        }
        public byte[] InfoToData()
        {
            return BitConverter.GetBytes(id.Length)
                .Concat(Encoding.ASCII.GetBytes(id))
                .Concat(BitConverter.GetBytes(position.x))
                .Concat(BitConverter.GetBytes(position.y))
                .Concat(BitConverter.GetBytes(rotation)).ToArray();
        }
        public Player(IPAddress ip, int playerPort, int recievePort)
        {
            valid = true;
            this.ip = ip;
            port = playerPort;
            id = recievePort.ToString();
            this.position = Vector2.zero;
            this.rotation = 0;
            con = new Connection(IPAddress.Any, recievePort, Handle, delay);
        }
        public void Send(byte[] data)
        {
            con.Send(data, ip, port);
        }
        public void Handle(byte[] data, IPAddress ip, int port)
        {
            if (!ip.Equals(this.ip))
            {
                Debug.Log("Error sending info to player");
                return;
            }
            Command cmd = (Command)BitConverter.ToInt32(data, 0);
            switch (cmd)
            {
                case Command.Connect:
                    this.port = port;
                    break;
                case Command.Send:
                    position = DataToVec2(data, 4);
                    rotation = BitConverter.ToSingle(data, 12);
                    if (myController != null)
                    {
                        myController.UpdatePos(position, rotation);
                    }
                    break;
                case Command.Quit:
                    valid = false;
                    break;
            }
        }
    }
}
