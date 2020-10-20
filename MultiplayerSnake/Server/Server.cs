using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Server
{
    class Server
    {
        private TcpListener listener;
        private static List<Client> clients;
        private static List<Account> accounts;

        static void Main(string[] args)
        {
            Console.WriteLine("Server start");
            new Server().StartListen();
        }

        /*
         * This method initializes the server to listen for clients.
         */
        public void StartListen()
        {
            accounts = FileReadWriter.RetrieveAllAccounts();
            if(accounts.Count <= 0) // test
                LoadTestAccounts(); // test
            clients = new List<Client>();
            listener = new TcpListener(IPAddress.Any, 1330);
            listener.Start();
            listener.BeginAcceptTcpClient(new AsyncCallback(Connect), null);

            string input = "";
            while(input != "quit")
            {
                Console.WriteLine("Commands:" +
                    "\n- quit");
                input = Console.ReadLine();
                switch (input)
                {
                    case "quit":
                        FileReadWriter.SaveAllAccounts(accounts);
                        break;
                    default:
                        break;
                }
            }
        }

        // Loads hardcoded accounts for testing.
        private void LoadTestAccounts()
        {
            accounts.Add(new Account(GenerateUniqueID(), "Kees", "123"));
            accounts.Add(new Account(GenerateUniqueID(), "Piet", "123"));
        }

        /*
         * Generates an random unique ID for a new account.
         */
        private string GenerateUniqueID()
        {
            Random random = new Random();
            string id = "";
            for (int i = 0; i < 10; i++)
            {
                id += Convert.ToChar(random.Next(48, 122));
            }
            return id;
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
            client.stream.Close();
            Console.WriteLine($"Client disconnected");
        }

        /*
         * Sends a package to all connected clients.
         */
        internal static void Broadcast(byte[] bytes)
        {
            foreach (var client in clients)
                client.stream.Write(bytes, 0, bytes.Length);
        }

        /*
         * Checks if received login credentials exits and are correct.
         */
        internal static bool CheckCredentials(string username, string password)
        {
            foreach(var acc in accounts)
            {
                if(acc.Username == username) // Check if username exits
                {
                    if(acc.Password == password) // Check if passwords match
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
