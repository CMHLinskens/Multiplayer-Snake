using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Utils;

namespace Server
{
    class Server
    {
        private TcpListener listener;
        private static List<Client> clients;
        private static List<Account> accounts;
        private static List<Account> availableAccounts;
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
            if (accounts.Count <= 0) // test
                LoadTestAccounts(); // test
            availableAccounts = new List<Account>(accounts);
            lobbies = new List<Lobby>();

            clients = new List<Client>();
            listener = new TcpListener(IPAddress.Any, 10001);
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
        internal static void DisconnectClient(Client client)
        {
            if (client.Lobby != null) LeaveLobby(client.Lobby.Name, client.Account.Username, client);
            if (client.Account != null) availableAccounts.Add(client.Account);
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
        internal static bool AddAccount(string username, string password)
        {
            if(AccountExists(username)) { return false; }
            Account newAccount = new Account(GenerateUniqueID(), username, password);
            accounts.Add(newAccount);
            availableAccounts.Add(newAccount);
            return true;
        }

        private static bool AccountExists(string username)
        {
            foreach (var acc in accounts)
                if (acc.Username == username)
                    return false;

            return true;
        }

        /*
         * Sends a package to all connected clients.
         */
        internal static void BroadcastToLobby(Lobby lobby, byte[] bytes)
        {
            foreach (var player in lobby.Players)
                GetClientWithUserName(player.Name).SendPacket(bytes);
        }

        /*
         * Creates a new lobby.
         */
        internal static bool CreateLobby(string lobbyName, string gameOwner, int maxPlayers, MapSize mapSize, Client client)
        {
            if (client.Lobby != null) { return false; } // Client is already in a lobby.

            foreach (var lobby in lobbies)
                if (lobby.Name == lobbyName)
                    // Lobby with this name already exists.
                    return false;

            Lobby newLobby = new Lobby(lobbyName, gameOwner, maxPlayers, mapSize);
            lobbies.Add(newLobby);
            client.Lobby = newLobby;
            Console.WriteLine(lobbies[lobbies.Count-1]);
            return true;
        }

        /*
         * Connects the Client to the lobby.
         */
        internal static bool JoinLobby(string lobbyName, string playerName, Client client)
        {
            if(client.Lobby != null) { return false; } // Client is already in a lobby.

            foreach (var lobby in lobbies)
                if (lobby.Name == lobbyName)
                {
                    bool joinResult = lobby.AddPlayer(playerName);
                    client.Lobby = joinResult ? lobby : null;
                    return joinResult;
                }

            // Lobby does not exist.
            return false;
        }

        /*
         * Disconnects the Client from the lobby.
         * If the client is the current game owner of the lobby, pass it to the next player in de list.
         */
        internal static bool LeaveLobby(string lobbyName, string playerName, Client client)
        {
            if (client.Lobby == null) return false; // Client is not in any lobby.

            foreach (var lobby in lobbies)
                if (lobby.Name == lobbyName)
                {
                    bool leaveResult = lobby.RemovePlayer(playerName);
                    if (leaveResult)
                    {
                        client.Lobby = null;
                        if (lobby.GameOwner == playerName) // Check if this client was the game owner.
                        {
                            if (lobby.Players.Count > 0) // Check if there are any players left in this lobby.
                            {
                                string newGameOwner = lobby.Players[0].Name;
                                // Send message to client to notify that he is the new game owner.
                                GetClientWithUserName(newGameOwner).SendPacket(PackageWrapper.SerializeData("newOwner", new { }));
                                lobby.GameOwner = newGameOwner;
                            }
                            else // If there is no-one left in the lobby, delete it.
                            {
                                lobby.DeleteGame();
                                lobbies.Remove(lobby);
                            }
                        }
                    }
                    return leaveResult;
                }
            // Client is not in this lobby.
            return false;
        }

        /*
         * Start the game of the given lobby.
         */
        internal static void StartGame(Client client)
        {
            if (client.Lobby != null)
                if (client.Lobby.GameOwner == client.Account.Username)
                    Task.Run(() => client.Lobby.StartGame());
        }

        /*
         * Stop the game of the given lobby.
         */
        internal static void StopGame(Client client)
        {
            if (client.Lobby != null)
                if (client.Lobby.GameOwner == client.Account.Username)
                {
                    client.Lobby.Game.StopGame();
                }
        }

        /*
         * Helper function for getting a client by name
         */
        internal static Client GetClientWithUserName(string username)
        {
            foreach (var client in clients)
                if (client.Account.Username == username)
                    return client;
            return null; // No client with this account connected.
        }

        /*
         * Start sending the lobbies in fragments to client.
         */
        internal static void StartSendLobbyList(Client client)
        {
            client.tempRefreshLobbyList = new List<Lobby>(lobbies);
            SendLobbyListFragment(client);
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
        internal static bool CheckCredentials(string username, string password, Client client)
        {
            foreach(var acc in availableAccounts)
            {
                if(acc.Username == username) // Check if username exits
                {
                    if(acc.Password == password) // Check if passwords match
                    {
                        client.Account = acc;
                        availableAccounts.Remove(acc);
                        return true; // Match found
                    }
                }
            }
            return false; // No match found
        }
    }
}
