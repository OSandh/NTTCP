using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace NTTCP
{
    class ClientConsole
    {
        User User { get; set; }
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

                    Console.WriteLine("{0}", msg);
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
                User = GetUserInfo();

                SendUserData(User);
                
                StreamWriter sWriter;

                sWriter = new StreamWriter(Client.GetStream());
                
                string msg;

                while (true)
                {
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

        private void SendUserData(User user)
        {
            byte[] userBytes;

            var bFormatter = new BinaryFormatter();

            using (var mStream = new MemoryStream())
            {
                bFormatter.Serialize(mStream, User);
                userBytes = mStream.ToArray();
            }

            BinaryWriter bWriter;

            bWriter = new BinaryWriter(Client.GetStream());

            Int32 userBytesLength = userBytes.Length;

            bWriter.Write(userBytesLength);
            bWriter.Write(userBytes);
            bWriter.Flush();
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
