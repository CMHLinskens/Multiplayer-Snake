using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Utils;

namespace Server
{
    class Server
    {
        private TcpListener listener;
        private static List<Client> clients;
        private static List<Account> accounts;
        private static List<Lobby> lobbies;

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
            lobbies = new List<Lobby>();

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
        private static string GenerateUniqueID()
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
            try
            {
                clients.Remove(client);
                client.stream.Close();
            }
            catch (IndexOutOfRangeException) { }
            Console.WriteLine($"Client disconnected");
        }

        /*
         * Adds an new account to the server.
         */
        internal static void AddAccount(string username, string password)
        {
            accounts.Add(new Account(GenerateUniqueID(), username, password));
        }

        /*
         * Sends a package to all connected clients.
         */
        internal static void Broadcast(byte[] bytes)
        {
            foreach (var client in clients)
                client.SendPacket(bytes);
        }

        /*
         * Creates a new lobby.
         */
        internal static bool CreateLobby(string lobbyName, string gameOwner)
        {
            foreach(var lobby in lobbies)
                if (lobby.Name == lobbyName)
                    // Lobby with this name already exists.
                    return false;

            lobbies.Add(new Lobby(lobbyName, gameOwner, 4, Utils.MapSize.size16x16));
            Console.WriteLine(lobbies[lobbies.Count-1]);
            return true;
        }

        /*
         * Connects the new player to the lobby.
         */
        internal static bool JoinLobby(string lobbyName, string playerName)
        {
            foreach (var lobby in lobbies)
                if (lobby.Name == lobbyName)
                    return lobby.AddPlayer(playerName);

            // Lobby does not exist
            return false;
        }

        /*
         * Start sending the lobbies in fragments to client.
         */
        internal static void StartSendLobbyList(Client client)
        {
            client.tempRefreshLobbyList = new List<Lobby>(lobbies);
            SendLobbyListFragment(client);
            //byte[] bytes = PackageWrapper.SerializeData("refresh/success", new { lobbies = lobbies });
            
        }

        /*
         * Sends a fragment of 2 lobbies to the client.
         * If there are 2 or less left, send /success message with the last lobbies.
         */
        internal static void SendLobbyListFragment(Client client)
        {
            if(client.tempRefreshLobbyList.Count > 2)
            {
                Lobby[] lobbyListFragment = { client.tempRefreshLobbyList[0], client.tempRefreshLobbyList[1] };
                client.SendPacket(PackageWrapper.SerializeData("refresh/fragment", new { lobbies = lobbyListFragment }));
                client.tempRefreshLobbyList.RemoveAt(0);
                client.tempRefreshLobbyList.RemoveAt(1);
            }
            else
            {
                client.SendPacket(PackageWrapper.SerializeData("refresh/success", new { lobbies = client.tempRefreshLobbyList }));
                client.tempRefreshLobbyList.Clear();
            }
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
                        return true; // Match found
                    }
                }
            }
            return false; // No match found
        }
    }
}
