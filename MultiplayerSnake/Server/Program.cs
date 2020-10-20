using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Server
{
    class Program
    {
        private TcpListener listener;
        private static List<Client> clients;
        private static Dictionary<string, string> savedAccounts; // <username, password>

        static void Main(string[] args)
        {
            Console.WriteLine("Server start");
            new Program().StartListen();
        }

        /*
         * This method initializes the server to listen for clients.
         */
        public void StartListen()
        {
            savedAccounts = LoadTestAccounts();
            clients = new List<Client>();
            listener = new TcpListener(IPAddress.Any, 1330);
            listener.Start();
            listener.BeginAcceptTcpClient(new AsyncCallback(Connect), null);
            Console.ReadLine();
        }

        // Loads hardcoded accounts for testing.
        private Dictionary<string, string> LoadTestAccounts()
        {
            Dictionary<string, string> accounts = new Dictionary<string, string>();
            accounts.Add("Kees", "123");
            accounts.Add("Frank", "123");
            accounts.Add("Piet", "123");
            return accounts;
        }

        /*
         * This method handles a new connection to the server.
         */
        private void Connect(IAsyncResult ar)
        {
            TcpClient tcpClient = listener.EndAcceptTcpClient(ar);
            Console.WriteLine($"Client connected from {tcpClient.Client.RemoteEndPoint}");
            clients.Add(new Client(tcpClient));
            listener.BeginAcceptTcpClient(new AsyncCallback(Connect), null);
        }

        /*
         * Removes client from server.
         */
        internal static void Disconnect(Client client)
        {
            clients.Remove(client);
            Console.WriteLine($"Client disconnected");
        }

        /*
         * Sends a package to all connected clients.
         */
        internal static void Broadcast(byte[] bytes)
        {
            foreach (var client in clients)
                client.GetStream().Write(bytes, 0, bytes.Length);
        }

        /*
         * Checks if received login credentials exits and are correct.
         */
        internal static bool CheckCredentials(string username, string password)
        {
            foreach(var name in savedAccounts.Keys)
            {
                if(name == username) // Check if username exits
                {
                    if(savedAccounts.GetValueOrDefault(username, "") == password) // Check if passwords match
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
