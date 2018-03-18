using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;


namespace NTTCP
{
    public class NTClient
    {
        #region Properties
        public User User { get; set; }
        private NTServer Server { get; set; }
        public string Name { get; set; }
        public StreamWriter SWriter { get; set; }
        public StreamReader SReader { get; set; }
        public XmlReader xReader { get; set; }
        private Thread ClientThread { get; set; }
        public TcpClient Connection { get; set; }
        public NTClient Partner { get; set; }
        #endregion 

        public NTClient(NTServer server, TcpClient client)
        {
            Server = server;

            Connection = client;

            Name = Connection.Client.RemoteEndPoint.ToString();

            ClientThread = new Thread(ReadClient)
            {
                Name = "ClientThread",
                IsBackground = true
            };
            ClientThread.Start();
        }

        private void ReadClient()
        {
            try
            {
                SWriter = new StreamWriter(Connection.GetStream());
                SReader = new StreamReader(Connection.GetStream());

                xReader = XmlReader.Create(Connection.GetStream());
                XmlSerializer xSerializer = new XmlSerializer(typeof(User));
                User = (User) xSerializer.Deserialize(xReader);

                Name = User.Name + ", " + User.Age;

                string msg = "";

                while (true)
                {
                    msg = SReader.ReadLine();

                    Server.LastClientMessage = msg;
                    Server.MessageLog.Enqueue(Name + ": " + msg);

                    Partner?.SWriter.WriteLine(msg);
                    Partner?.SWriter.Flush();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            
        }

        public void SendMessage(string msg)
        {
            SWriter?.WriteLine(msg);
            SWriter?.Flush();
        }
        
    }
}