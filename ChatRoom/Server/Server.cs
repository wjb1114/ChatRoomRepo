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
        Dictionary<string, string> connectedUsers;
        Queue<byte[]> messageQueue;
        public void RunServer()
        {
            connectedUsers = new Dictionary<string, string>();
            messageQueue = new Queue<byte[]>();
            server = new WatsonTcpServer("192.168.209.7", 9000);
            server.ClientConnected = ClientConnected;
            server.ClientDisconnected = ClientDisconnected;
            server.MessageReceived = MessageReceived;
            server.Debug = false;
            server.Start();

            bool runForever = true;
            while (runForever)
            {
                Console.WriteLine("Command [q]: ");
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
            string disconnectingUsername = "";
            foreach(KeyValuePair<string, string> users in connectedUsers)
            {
                if (users.Value == ipPort)
                {
                    disconnectingUsername = users.Key;
                }
            }
            if (disconnectingUsername != "")
            {
                connectedUsers.Remove(disconnectingUsername);
                return true;
            }
            else
            {
                return false;
            }

        }

        bool MessageReceived(string ipPort, byte[] data)
        {
            string[] identification;
            string msg = "";
            bool isMessage;
            if (data != null && data.Length > 0)
            {
                msg = Encoding.UTF8.GetString(data);
                identification = msg.Split('|');
                if (identification[0] == "msg")
                {
                    isMessage = true;
                }
                else
                {
                    isMessage = false;
                }
            }
            else
            {
                return false;
            }

            if (isMessage == false)
            {
                connectedUsers.Add(identification[1], ipPort);
                messageQueue.Enqueue(Encoding.UTF8.GetBytes(identification[1] + " joined the chat."));
            }
            else
            {
                string currentUser = "";
                foreach(KeyValuePair<string, string> user in connectedUsers)
                {
                    if (user.Value == ipPort)
                    {
                        currentUser = user.Key;
                        break;
                    }
                }
                if (currentUser == "")
                {
                    return false;
                }
                else
                {
                    string fullMessage = currentUser + ": " + identification[1];
                    messageQueue.Enqueue(Encoding.UTF8.GetBytes(fullMessage));
                }
                
            }
            Console.WriteLine("Message received from " + ipPort + ": " + msg);
            new Task(ProcessMessageQueue).Start();
            return true;
        }
        void ProcessMessageQueue()
        {
            if (messageQueue.Count <= 0)
            {
                return;
            }
            else
            {
                while(messageQueue.Count > 0)
                {
                    byte[] messageToSend = messageQueue.Dequeue();
                    foreach(KeyValuePair<string, string> users in connectedUsers)
                    {
                        server.Send(users.Value, messageToSend);
                    }
                }
            }
        }
    }
}
