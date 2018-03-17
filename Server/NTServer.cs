﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NTTCP
{
    /// <summary>
    /// NTServer is obviously the TCP-Server of the NTTCP library.
    /// WIP
    /// </summary>
    public class NTServer
    {
        #region Properties
        private Thread ServerThread { get; set; }
        private TcpListener Listener { get; set; }
        public IPAddress IP { get; set; }
        public string HostName { get; set; }
        public List<NTClient> ClientList { get; set; } = new List<NTClient>();
        public int Port { get; set; }
        public bool IsLAN { get; set; }

        #endregion

        public NTServer()
        {
            IP = IPAddress.Parse("192.168.1.123");
            Port = 27015;
        }

        private IPAddress GetLocalIP()
        {
            return IPAddress.Any;
        }

        public bool StartServer()
        {
            try
            {
                ServerThread = new Thread(Listen);
                ServerThread.Name = "ServerThread";
                ServerThread.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine("Cannot start Thread: {0}\n{1}", ServerThread.Name, e.Message);
                return false;
            }

            return true;
        }

        private void Listen()
        {
            try
            {
                Listener = new TcpListener(IP, Port);
                Listener.Start();

                while (true)
                {
                    Console.WriteLine("Waiting for connection...");

                    TcpClient tcpC = Listener.AcceptTcpClient();

                    NTClient client = new NTClient(tcpC);

                    ClientList.Add(client);

                    Console.WriteLine("{0} connected!", tcpC.Client.ToString());

                    if (ClientList.Count == 2)
                        PairClients();
                }
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                Listener?.Stop();
                KillClients();
                ServerThread.Abort();
            }
        }

        private void PairClients()
        {
            ClientList[0].Partner = ClientList[1];
            ClientList[1].Partner = ClientList[0];
        }

        private void KillClients()
        {
            foreach(var client in ClientList)
            {
                client?.Connection?.Close();
            }
        }

        public void SendMessage(string input)
        {
            foreach(var client in ClientList)
            {
                client.SendMessage(input);
            }
        }
    }
}
