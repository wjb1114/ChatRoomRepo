using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatsonTcp;

namespace Server
{
    class Server
    {
        WatsonTcpServer server;
        public void RunServer()
        {
            server = new WatsonTcpServer("127.0.0.1", 9000);
            server.ClientConnected = ClientConnected;
            server.ClientDisconnected = ClientDisconnected;
            server.MessageReceived = MessageReceived;
            server.Debug = false;
            server.Start();

            bool runForever = true;
            while (runForever)
            {
                Console.Write("Command [q]: ");
                string userInput = Console.ReadLine();
                if (string.IsNullOrEmpty(userInput)) continue;

                switch (userInput)
                {
                    case "q":
                        runForever = false;
                        break;
                }
            }
        }

        bool ClientConnected(string ipPort)
        {
            Console.WriteLine("Client connected: " + ipPort);
            return true;
        }

        bool ClientDisconnected(string ipPort)
        {
            Console.WriteLine("Client disconnected: " + ipPort);
            return true;
        }

        bool MessageReceived(string ipPort, byte[] data)
        {
            List<string> clients;
            clients = server.ListClients();
            string msg = "";
            if (data != null && data.Length > 0) msg = Encoding.UTF8.GetString(data);
            foreach (string curr in clients)
            {
                server.Send(curr, data);
            }
            Console.WriteLine("Message received from " + ipPort + ": " + msg);
            return true;
        }
    }
}
