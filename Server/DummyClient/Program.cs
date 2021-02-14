﻿using DummyClient.Session;
using ServerCore;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace DummyClient
{
    
    
    class Program
    {
        static int DummyClientCount { get; } = 300;
        static void Main(string[] args)
        {
            Thread.Sleep(4000);
            // DNS (Domain Name System)
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

            Connector connector = new Connector();

            connector.Connect(endPoint,
                () => { return SessionManager.Instance.Generate(); },
                DummyClientCount);

            while (true)
            {
                Thread.Sleep(10000);
            }
        }
    }
}
