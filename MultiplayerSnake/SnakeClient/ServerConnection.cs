using Microsoft.CSharp.RuntimeBinder;
using Microsoft.VisualBasic.CompilerServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SnakeClient.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace SnakeClient
{
    class ServerConnection
    {
        private TcpClient tcpClient;
        private string ipAddress = "127.0.0.1";
        private int port = 1330;
        private int totalConnectTries;
        private readonly int maxReconnectingTries = 3;
        private byte[] buffer = new byte[1024];
        private ObservableCollection<Lobby> lobbyListBuilder;
        public bool LoggedIn { get; private set; }
        public ObservableCollection<Lobby> Lobbies { get { return lobbyListBuilder; } }
        public Lobby joinedLobby { get; private set; }
        public Direction MoveDirection { get; set; }
        public int[,] GameField { get; set; }

        #region // Reply booleans
        public bool ReceivedLoginMessage { get; private set; }
        public bool ReceivedLobbyCreateMessage { get; private set; }
        public bool ReceivedLobbyJoinMessage { get; private set; }
        public bool ReceivedAllLobbies { get; private set; }
        public bool ReceivedLobbyLeaveMessage { get; private set; }
        public bool ReceivedLobbyRefresh { get; private set; }
        public bool ReceivedStartGame { get; set; }
        public bool ReceivedNewUpdate { get; set; }
        public bool ReceivedGameStartMessage { get; set; }
        #endregion

        public ServerConnection()
        {
            tcpClient = new TcpClient();
        }

        /*
         * Connects the client to the server.
         * If unsuccessful try again for 3 times.
         */
        private void Connect(string ipAddress, int port)
        {
            try
            {
                tcpClient = new TcpClient();
                tcpClient.Connect(ipAddress, port);

                if (tcpClient.Connected)
                    OnConnected();
            }
            catch (Exception)
            {
                if (!tcpClient.Connected && totalConnectTries < maxReconnectingTries)
                {
                    totalConnectTries++;
                    Connect(ipAddress, port);
                }
                else
                    Disconnect();
            }
        }

        /*
         * Gets called when connecting to server is successful
         */
        private void OnConnected()
        {
            tcpClient.GetStream().BeginRead(buffer, 0, buffer.Length, new AsyncCallback(OnRead), null);
        }

        /*
         * Gets called when connecting to server fails and when connection is lost.
         */
        private void Disconnect()
        {
            LoggedIn = false;
            try
            {
                tcpClient.GetStream().Dispose();
                tcpClient.Close();
            }
            catch (ObjectDisposedException) {}
            catch (InvalidOperationException) {}
        }

        /*
         * Method that gets called when the stream is reading something.
         */
        private void OnRead(IAsyncResult ar)
        {
            try
            {
                int receivedBytes = tcpClient.GetStream().EndRead(ar);
                string receivedText = Encoding.ASCII.GetString(buffer, 0, receivedBytes);
                dynamic receivedData = JsonConvert.DeserializeObject(receivedText);
                HandleData(receivedData);
                tcpClient.GetStream().BeginRead(buffer, 0, buffer.Length, new AsyncCallback(OnRead), null);
            }
            catch (IOException)
            {
                Disconnect();
            }
            catch (ObjectDisposedException)
            {
                Disconnect();
            }
        }

        /*
         * This method handles all the incoming data received by the OnRead method.
         */
        private void HandleData(dynamic data)
        {
            string tag = data.tag;
            switch (tag)
            {
                case "chat":
                    Console.WriteLine($"{data.data.message}");
                    break;
                case "login/success":
                    LoggedIn = true;
                    ReceivedLoginMessage = true;
                    Console.WriteLine($"{data.data.message}");
                    break;
                case "login/error":
                    ReceivedLoginMessage = true;
                    Disconnect();
                    Console.WriteLine($"{data.data.message}");
                    break;
                case "create/success":
                    // Created lobby on server.
                    ReceivedLobbyCreateMessage = true;
                    break;
                case "create/error":
                    // Unable to create lobby on server.
                    ReceivedLobbyCreateMessage = true;
                    break;
                case "join/success":
                    // Joined the lobby on server.
                    ReceivedLobbyJoinMessage = true;
                    break;
                case "join/error":
                    // Unable to join lobby.
                    ReceivedLobbyJoinMessage = true;
                    break;
                case "leave/success":
                    // Left the lobby on server.
                    ReceivedLobbyLeaveMessage = true;
                    break;
                case "leave/error":
                    // Unable to leave the lobby.
                    ReceivedLobbyLeaveMessage = true;
                    break;
                case "refresh/fragment":
                    // Received a fragment with 2 lobbies inside.
                    AddLobbies(data.data.lobbies);
                    GetNextLobbyListFragment();
                    break;
                case "refresh/success":
                    // Received last of the lobby list.
                    AddLobbies(data.data.lobbies);
                    ReceivedAllLobbies = true;
                    break;
                case "refresh/lobby/success":
                    // Received requested lobby.
                    RefreshJoinedLobby(data.data.lobby);
                    ReceivedLobbyRefresh = true;
                    break;
                case "newOwner":
                    // This client is the new owner of the lobby.
                    break;
                case "game/start/success":
                    // Game has started.
                    ReceivedGameStartMessage = true;
                    break;
                case "game/move/request":
                    // Send the next desired move to the server.
                    // TODO
                    // retrieve the desired move direction.
                    SendNextMove(MoveDirection);
                    break;
                case "game/update":
                    // Recieved new update of the active game.
                    ReceivedNewUpdate = true;
                    UpdateGameField(data.data.gameField);
                    break;
                case "game/end":
                    // The game has been won by someone

                default:
                    Console.WriteLine($"No handling found for tag: {tag}");
                    break;
            }
        }

        /*
         * Helper function to build up the new GameField
         */
        private void UpdateGameField(dynamic gameField)
        {
            JArray gameFieldRows = gameField as JArray;
            for (int row = 0; row < gameField.Count; row++)
            {
                for (int column = 0; column < gameField.Count; column++)
                {
                    GameField[row, column] = gameFieldRows[row][column].ToObject<int>();
                }
            }
        }

        /*
         * Helper function to refresh joined lobby.
         */
        private void RefreshJoinedLobby(dynamic lobby)
        {
            try // only epics use try catch
            {
                ObservableCollection<Player> players = new ObservableCollection<Player>();
                foreach (dynamic player in ((JArray)lobby.Players).Children())
                    players.Add(new Player((string)player.Name));
                joinedLobby = new Lobby((string)lobby.Name, players, (bool)lobby.IsInGame, (int)lobby.MaxPlayers, (string)lobby.GameOwner, (MapSize)lobby.MapSize);
            }
            catch(RuntimeBinderException) { }
        }

        /*
         * Helper function to build up the lobby list.
         */
        private void AddLobbies(dynamic newLobbies)
        {
            foreach (dynamic lobby in ((JArray)newLobbies).Children())
            {
                ObservableCollection<Player> players = new ObservableCollection<Player>();
                foreach (dynamic player in ((JArray)lobby.Players).Children())
                    players.Add(new Player((string)player.Name));
                lobbyListBuilder.Add(new Lobby((string)lobby.Name, players, (bool)lobby.IsInGame, (int)lobby.MaxPlayers, (string)lobby.GameOwner, (MapSize)lobby.MapSize));
            }
        }

        #region // Writer functions
        /*
         * Helper function for sending packets to server.
         */
        private void SendPacket(byte[] bytes)
        {
            tcpClient.GetStream().Write(bytes, 0, bytes.Length);
        }
        /*
         * Send chat message to server.
         */
        public void SendChat(string input)
        {
            SendPacket(PackageWrapper.SerializeData("chat", new { message = input }));
        }
        /*
         * Sends the new user credentials to the server to create a new account.
         */
        public void Register(string username, string password)
        {
            Connect(ipAddress, port);
            SendPacket(PackageWrapper.SerializeData("register", new { username = username, password = password }));
        }
        /*
         * Sends the user credentials to the server to login.
         */
        public void Login(string username, string password)
        {
            Connect(ipAddress, port);
            ReceivedLoginMessage = false;
            SendPacket(PackageWrapper.SerializeData("login", new { username = username, password = password }));
        }
        /*
         * Creates a new lobby at the server.
         */
        public void CreateLobby(string lobbyName, string gameOwner, int maxPlayers, MapSize mapSize)
        {
            ReceivedLobbyCreateMessage = false;
            SendPacket(PackageWrapper.SerializeData("create", new { lobbyName = lobbyName, gameOwner = gameOwner, maxPlayers = maxPlayers, mapSize = mapSize }));
        }
        /*
         * Connects the client to the lobby with the same name.
         */
        public void ConnectToLobby(string lobbyName, string playerName)
        {
            ReceivedLobbyJoinMessage = false;
            SendPacket(PackageWrapper.SerializeData("join", new { lobbyName = lobbyName, playerName = playerName }));
        }
        /*
         * Disconnects the client from the lobby with the same name.
         */
        public void LeaveLobby(string lobbyName, string playerName)
        {
            ReceivedLobbyLeaveMessage = false;
            SendPacket(PackageWrapper.SerializeData("leave", new { lobbyName = lobbyName, playerName = playerName }));
        }
        /*
         * Notifies the server to send current list of active lobbies to this client.
         */
        public void RefreshLobbyList()
        {
            ReceivedAllLobbies = false;
            lobbyListBuilder = new ObservableCollection<Lobby>();
            SendPacket(PackageWrapper.SerializeData("refresh", new { }));
        }
        /*
         * Asks the server to send the updated lobby to this client.
         */
        public void RefreshSpecificLobby()
        {
            ReceivedLobbyRefresh = false;
            SendPacket(PackageWrapper.SerializeData("refresh/lobby", new { }));
        }
        /*
         * Asks the server the send the next fragment of the list.
         */
        public void GetNextLobbyListFragment()
        {
            SendPacket(PackageWrapper.SerializeData("refresh/next", new { }));
        }
        /*
         * Start the game of the lobby that this client is in.
         */
        public void StartGame()
        {
            SendPacket(PackageWrapper.SerializeData("game/start", new { }));
        }
        /*
         * Stop the game of the lobby that this client is in.
         */
        public void StopGame()
        {
            SendPacket(PackageWrapper.SerializeData("game/stop", new { }));
        }
        /*
         * Sends the next desired move to the server.
         */
        public void SendNextMove(Direction move)
        {
            SendPacket(PackageWrapper.SerializeData("game/move/response", new { move = move }));
        }
        #endregion
    }
}
