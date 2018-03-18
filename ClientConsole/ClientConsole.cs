using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace NTTCP
{
    class ClientConsole
    {
        User user;
        private TcpClient Client { get; set; }
        private Thread ReadThread { get; set; }
        private Thread WriteThread { get; set; }

        public ClientConsole()
        {
            try
            {
                Client = new TcpClient("192.168.1.162", 27015);
            }
            catch (SocketException e)
            {
                Console.WriteLine(e.Message);
                Environment.Exit(1);
            }

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
                XmlWriter xWriter = XmlWriter.Create(Client.GetStream());

                string msg;

                user = GetUserInfo();

                XmlSerializer xSerializer = new XmlSerializer(typeof(User));
                xSerializer.Serialize(xWriter, user);
                
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

        private User GetUserInfo()
        {
            Console.WriteLine("Enter username\n");
            Console.Write('>');

            string name = Console.ReadLine();

            int age;

            do
            {
                Console.WriteLine("Enter Age\n");
                Console.Write('>');
            } while (!int.TryParse(Console.ReadLine(), out age));

            return new User(name, age);
        }
    }
}
