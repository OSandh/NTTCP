using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NTTCP;

namespace ServerConsole
{

    class ServerConsole
    {
        public NTServer Server { get; set; }
        public Thread ReadThread { get; set; }
        public Thread WriteThread { get; set; }
        public bool IsServerOnline { get; private set; }

        #region ValidMenuInputs
        string[] startInputs =
        {
                "1",
                "1.",
                "start",
                "start server"
        };

        string[] stopInputs =
        {
                "1",
                "1.",
                "stop",
                "stop server"
        };

        string[] secondInputs =
        {
                "2",
                "2.",
                "view",
                "view clients"
        };

        string[] zeroInputs =
        {
                "0",
                "0.",
                "quit",
                "exit",
                "shutdown"
        };
        #endregion

        public ServerConsole()
        {
            try
            {
                ReadThread = new Thread(Read)
                {
                    Name = "ReadThread",
                };

                WriteThread = new Thread(Write)
                {
                    Name = "WriteThread",
                };

                ReadThread.Start();
               // WriteThread.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            
        }

        private void Read()
        {
            try
            {
                while (true)
                {
                    Console.SetCursorPosition(0, 5);
                    PrintMenu();
                    Console.Write("> ");
                    string input = Console.ReadLine();

                    if (IsValidInput(input, startInputs) && !IsServerOnline)
                    {
                        Start();
                    }
                    else if(IsValidInput(input, stopInputs))
                    {
                        Stop();
                    }
                    else if(IsValidInput(input, secondInputs))
                    {
                        ViewClients();
                    }
                    else if(IsValidInput(input, zeroInputs))
                    {
                        Shutdown();
                    }
                    else if(input.StartsWith("send "))
                    {
                        Server?.SendMessage(input.Substring(5));
                    }
                    else
                    {
                        Console.WriteLine("Invalid input");
                    }
                    
                    Console.WriteLine("\nPress any key to continue");
                    Console.ReadLine();

                    // clear menu part of console
                    Console.SetCursorPosition(0, 5);
                    for (int i = 0; i < 15; i++)
                    {
                        Console.Write(new string(' ', Console.WindowWidth));
                    }
                    
                }
                
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private void Write()
        {
            throw new NotImplementedException();
        }

        private void Stop()
        {
            Server?.Stop();
            Server = null;
            IsServerOnline = false;
        }

        private void Shutdown()
        {
            Stop();
            Environment.Exit(1);
        }

        private void ViewClients()
        {
            Console.WriteLine();

            foreach(var client in Server.ClientList)
            {
                Console.WriteLine(client.ToString());
            }
        }

        private void PrintMenu()
        {
            string firstItem = "Start Server";

            if (IsServerOnline)
                firstItem = "Stop Server";

            Console.WriteLine("\n1. "+ firstItem +"\n" +
                                "2. View Clients\n" +
                                "0. Quit\n" +
                                "To broadcast message type: send Example Message\n");
        }

        public bool Start()
        {
            Server = new NTServer();

            if (Server.Start())
            {
                IsServerOnline = true;
                return true;
            }
                
            
            return false;
        }

        private bool IsValidInput(string input, string[] compareWith)
        {
            foreach (var s in compareWith)
            {
                if (input == s)
                    return true;
            }

            return false;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            ServerConsole console = new ServerConsole();
        }
    }
}
