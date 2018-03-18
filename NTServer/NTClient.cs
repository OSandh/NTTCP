using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
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
        public BinaryReader BinaryReader { get; private set; }
        public XmlReader xReader { get; set; }
        private Thread ClientThread { get; set; }
        public TcpClient Connection { get; set; }
        public NTClient Partner { get; set; }
        #endregion 

        public NTClient(NTServer server, TcpClient client, User user)
        {
            Server = server;

            Connection = client;

            User = user;
            Name = User.Name + ", " + User.Age;

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

                //int userByteSize = int.Parse(SReader.ReadLine());

                //BinaryReader bReader;

                //bReader = new BinaryReader(Connection.GetStream());

                //byte[] userBytes = bReader.ReadBytes(userByteSize);

                //using (var mStream = new MemoryStream())
                //{
                //    var bFormatter = new BinaryFormatter();
                //    mStream.Write(userBytes, 0, userBytes.Length);
                //    mStream.Seek(0, SeekOrigin.Begin);
                //    User = (User) bFormatter.Deserialize(mStream);
                    
                //}
                
                //string userXml = SReader.ReadLine();


                ////var xWriter = XmlWriter.Create()

                ////xReader = XmlReader.Create(Connection.GetStream());
                //Serializer xSerializer = new Serializer();
                //////xReader.Read();
                //User = xSerializer.Deserialize<User>(userXml);


                string msg = "";

                while (true)
                {
                    msg = SReader.ReadLine();

                    Server.LastClientMessage = msg;
                    Server.MessageLog.Enqueue(Name + ": " + msg);

                    Partner?.SWriter.WriteLine("{0}: {1}", User.ToString(), msg);
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