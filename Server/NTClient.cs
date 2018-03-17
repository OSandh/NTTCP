using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NTTCP
{
    public class NTClient
    {
        #region Properties
        public StreamWriter SWriter { get; set; }
        public StreamReader SReader { get; set; }
        private Thread ClientReadThread { get; set; }
        public TcpClient Connection { get; set; }
        public NTClient Partner { get; set; }
        #endregion 

        public NTClient(TcpClient client)
        {
            Connection = client;
            ClientReadThread = new Thread(ReadClient);
            ClientReadThread.Name = "ClientThread";
            ClientReadThread.Start();
        }

        private void ReadClient()
        {
            try
            {
                SWriter = new StreamWriter(Connection.GetStream());
                SReader = new StreamReader(Connection.GetStream());

                string msg = "";

                while (true)
                {
                    msg = SReader.ReadLine();

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