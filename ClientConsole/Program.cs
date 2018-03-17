using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ClientConsole
{
    class RunClient
    {
        private TcpClient Client { get; set; }
        private Thread ReadThread { get; set; }
        private Thread WriteThread { get; set; }

        public RunClient()
        {
            Client = new TcpClient("192.168.1.123", 27015);
            ReadThread = new Thread(Read);
            ReadThread.Name = "ReadThread";
            ReadThread.Start();

            WriteThread = new Thread(Write);
            WriteThread.Name = "WriteThread";
            WriteThread.Start();
        }

        private void Read()
        {
            try
            {

                StreamReader sReader = new StreamReader(Client.GetStream());

                string msg = "";

                while (true)
                {
                    msg = sReader.ReadLine();

                    Console.WriteLine("Incoming msg: {0}", msg);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                Client?.Close();
                ReadThread.Abort();
            }
        }

        private void Write()
        {
            try
            {
                StreamWriter sWriter = new StreamWriter(Client.GetStream());

                string msg = "";

                while (true)
                {
                    Console.WriteLine("Send Message\n>");
                    msg = Console.ReadLine();
                    sWriter.WriteLine(msg);
                    sWriter.Flush();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                Client?.Close();
                WriteThread.Abort();
            }
        }


    }

    class Program
    {
        static void Main(string[] args)
        {
            RunClient run = new RunClient();
            
        }

        
    }
}
