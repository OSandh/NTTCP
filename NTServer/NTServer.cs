﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
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
        private bool Run { get; set; }
        public string LastClientMessage { get; set; }
        public Queue<string> MessageLog { get; set; } = new Queue<string>(10);

        #endregion

        public NTServer()
        {
            IP = GetLocalIP();
            Port = 27015;
        }

        /// <summary>
        /// Finds and stores the servers local ip-address
        /// </summary>
        /// <returns>IPAddress</returns>
        private IPAddress GetLocalIP()
        {
            IPAddress ip = null;

            IPHostEntry host;
            host = Dns.GetHostEntry(Dns.GetHostName());

            foreach (var _ip in host.AddressList)
            {
                if (_ip.AddressFamily == AddressFamily.InterNetwork)
                    ip = _ip;
            }

            return ip;
        }

        public bool Start()
        {
            try
            {
                ServerThread = new Thread(Listen);
                ServerThread.Name = "ServerThread";
                Run = true;
                ServerThread.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine("Cannot start Thread: {0}\n{1}", ServerThread.Name, e.Message);
                Run = false;
                return false;
            }

            return true;
        }

        public void Stop()
        {
            Listener?.Stop();
            Run = false;
        }

        private void Listen()
        {
            try
            {
                Listener = new TcpListener(IP, Port);
                Listener.Start();

                while (Run)
                {
                    // get current curson pos
                    int cursorTop = Console.CursorTop;
                    int cursorLeft = Console.CursorLeft;

                    Console.SetCursorPosition(0, 0);
                    Console.WriteLine("Waiting for connection...");

                    // reset curson pos
                    Console.SetCursorPosition(cursorLeft, cursorTop);

                    TcpClient tcpC = Listener.AcceptTcpClient();

                    User user = RecieveUserData(tcpC.GetStream());

                    if(user != null)
                    {
                        NTClient client = new NTClient(this, tcpC, user);

                        ClientList.Add(client);

                        Console.SetCursorPosition(0, 1);
                        Console.WriteLine("{0} connected!", tcpC.Client.ToString());

                        // reset curson pos
                        Console.SetCursorPosition(cursorLeft, cursorTop);

                        if (ClientList.Count == 2)
                            PairClients();
                    }
                    else
                    {
                        Console.SetCursorPosition(0, 1);
                        Console.WriteLine("Client rejected. Could not resolve client user data...");
                        Console.SetCursorPosition(cursorLeft, cursorTop);
                        tcpC?.Close();
                    }
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

        private User RecieveUserData(NetworkStream clientStream)
        {
            BinaryReader bReader;

            bReader = new BinaryReader(clientStream);

            // read size of byte incoming byte stream
            int userByteSize = bReader.ReadInt32();

            byte[] userBytes = bReader.ReadBytes(userByteSize);

            using (var mStream = new MemoryStream())
            {
                var bFormatter = new BinaryFormatter();
                mStream.Write(userBytes, 0, userBytes.Length);
                mStream.Seek(0, SeekOrigin.Begin);
                return (User)bFormatter.Deserialize(mStream);
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
                client.SendMessage("Server: " + input);
            }
            MessageLog.Enqueue("Server: " + input);
        }
    }
}
