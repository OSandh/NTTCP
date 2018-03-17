using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NTTCP;

namespace ServerConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            NTServer server = new NTServer();

            if (!server.StartServer())
                Environment.Exit(1);
            {
                string input = "";

                while (true)
                {
                    //Console.Clear();
                    Console.WriteLine("Send a message to the clients (or type quit to shut down server):\n>");
                    input = Console.ReadLine();

                    if (input == "quit")
                        break;
                    else
                    {
                        server.SendMessage("Message from server: " + input);
                    }
                }
            }
            

            Console.ReadLine();
        }
    }
}
