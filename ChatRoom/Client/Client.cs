using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WatsonTcp;

namespace Client
{
    class Client
    {
        string clientName;
        public void InitClient()
        {
            bool validUsername;
            Regex regex = new Regex("^[a-zA-Z0-9_-]+$");
            do
            {
                Console.WriteLine("Please enter your username:");
                clientName = Console.ReadLine();
                if (regex.IsMatch(clientName))
                {
                    validUsername = true;
                }
                else
                {
                    validUsername = false;
                }
            }
            while (validUsername == false);
            WatsonTcpClient client = new WatsonTcpClient("127.0.0.1", 9000);
            client.ServerConnected = ServerConnected;
            client.ServerDisconnected = ServerDisconnected;
            client.MessageReceived = MessageReceived;
            client.Debug = false;
            client.Start();

            bool runForever = true;
            while (runForever)
            {
                Console.WriteLine("Command [q cls send auth]: ");
                string userInput = Console.ReadLine();
                if (string.IsNullOrEmpty(userInput)) continue;

                switch (userInput)
                {
                    case "q":
                        runForever = false;
                        break;
                    case "cls":
                        Console.Clear();
                        break;
                    case "send":
                        Console.Write("Data: ");
                        userInput = Console.ReadLine();
                        if (string.IsNullOrEmpty(userInput)) break;
                        client.Send(Encoding.UTF8.GetBytes(clientName + ": " + userInput));
                        break;
                    case "auth":
                        Console.Write("Preshared key: ");
                        userInput = Console.ReadLine();
                        if (string.IsNullOrEmpty(userInput)) break;
                        client.Authenticate(userInput);
                        break;
                }
            }
        }

        bool MessageReceived(byte[] data)
        {
            Console.WriteLine("Message from server: " + Encoding.UTF8.GetString(data));
            return true;
        }

        bool ServerConnected()
        {
            Console.WriteLine("Server connected");
            return true;
        }

        bool ServerDisconnected()
        {
            Console.WriteLine("Server disconnected");
            return true;
        }
    }
}
