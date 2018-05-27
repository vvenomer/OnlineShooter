using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class Connection
{
    public delegate void HandleConnection(byte[] data, IPAddress ip, int port);
    private UdpClient listener;
    private IPEndPoint ep;
    private bool server;
    private HandleConnection Handle;
    public int Port
    {
        get { return ep.Port; }
        set
        {
            ep.Port = value;
            if (!server)
                listener.Connect(ep);
            else
            {
                ep.Address = IPAddress.Any;
                listener = new UdpClient(ep);
            }
        }
    }
    public Connection(IPAddress ip, int port, HandleConnection Handle, int listenDelay)
    {
        this.Handle = Handle;
        this.server = ip == IPAddress.Any;
        ep = new IPEndPoint(ip, port);
        if (ip != IPAddress.Any)
        {
            listener = new UdpClient();
            listener.Connect(ep);
        }
        else
        {
            listener = new UdpClient(ep);
        }
        new ProcFunc(Listener, listenDelay);
    }
    public int Send(byte[] data, IPAddress ip, int port)
    {
        return listener.Send(data, data.Length, new IPEndPoint(ip, port));
    }
    public int Send(byte[] data)
    {
        return listener.Send(data, data.Length);
    }
    void Listener()
    {
        if (listener.Available > 0)
        {
            byte[] response = listener.Receive(ref ep);
            new Thread(() => Handle(response, ep.Address, ep.Port)).Start();
        }
    }
}