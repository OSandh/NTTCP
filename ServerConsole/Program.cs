using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NTTCP;

namespace NTTCP
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

        string[] thirdInputs =
        {
                "3",
                "4.",
                "log",
                "view log"
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

        private void PrintMessageLog()
        {
            
            try
            {
                ClearMenuSegment();
                foreach (var s in Server.MessageLog)
                {
                    Console.WriteLine(s);
                }
            }
            catch (NullReferenceException )
            {
                Console.WriteLine("no messages");
            }

            Console.WriteLine("\nAny key to go back");
            Console.ReadLine();
        }

        private void Read()
        {
            try
            {
                while (true)
                {
                    Console.SetCursorPosition(0, 5);
                    PrintMenu();
                    HandleInput();

                    ClearMenuSegment();
                }
                
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private void ClearMenuSegment()
        {
            int cursorLeft = Console.CursorLeft;
            int cursorTop = Console.CursorTop;

            // clear menu part of console
            Console.SetCursorPosition(0, 5);

            int i = 0;
            while(i < 20)
            {
                Console.Write(new string(' ', Console.WindowWidth));
                Console.SetCursorPosition(0, 5 + i++);
            }
            Console.SetCursorPosition(cursorLeft, cursorTop);
        }

        private void HandleInput()
        {
            Console.SetCursorPosition(0, 15);
            Console.Write("> ");

            string input = Console.ReadLine();
            input = input.ToLower();

            if (IsValidInput(input, startInputs) && !IsServerOnline)
            {
                Start();
            }

            else if (IsValidInput(input, stopInputs))
            {
                Stop();
            }

            else if (IsValidInput(input, secondInputs))
            {
                ViewClients();
            }

            else if(IsValidInput(input, thirdInputs))
            {
                PrintMessageLog();
            }

            else if (IsValidInput(input, zeroInputs))
            {
                Shutdown();
            }

            else if (input.StartsWith("send "))
            {
                Server?.SendMessage(input.Substring(5));
            }

            else
            {
                Console.SetCursorPosition(0, 15);
                Console.WriteLine("Invalid input");
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
            ClearMenuSegment();

            Console.SetCursorPosition(0, 5);
            int i = 1;
            foreach(var client in Server.ClientList)
            {
                Console.WriteLine("{0}. {1}", i++, client.Name);
            }

            Console.WriteLine("\n\nAny key to go back");
            Console.ReadLine();
        }

        private void PrintMenu()
        {
            string firstItem = "Start Server";

            if (IsServerOnline)
                firstItem = "Stop Server";

            Console.WriteLine("\n1. "+ firstItem +"\n" +
                                "2. View Clients\n" +
                                "3. View log\n" +
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
