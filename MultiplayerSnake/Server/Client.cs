﻿using Microsoft.CSharp.RuntimeBinder;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using Utils;

namespace Server
{
    class Client
    {
        private TcpClient tcpClient;
        private byte[] buffer = new byte[1024];
        public List<Lobby> tempRefreshLobbyList { get; set; } // Use this list to temporary store all lobbies to send them in packets to client.
        public NetworkStream stream { get; }
        public Account Account { get; set; }
        public Lobby Lobby { get; set; }
        public bool ReceivedNextMove { get; set; }
        public Direction NextMove { get; private set; }

        public Client(TcpClient newTcpClient)
        {
            tcpClient = newTcpClient;
            stream = tcpClient.GetStream();
            // Start reading from the stream.
            stream.BeginRead(buffer, 0, buffer.Length, new AsyncCallback(OnRead), null);
        }

        /*
         * Method that gets called when the stream is reading something.
         */
        private void OnRead(IAsyncResult ar)
        {
            try
            {
                int receivedBytes = stream.EndRead(ar);
                string receivedText = Encoding.ASCII.GetString(buffer, 0, receivedBytes);
                dynamic data = JsonConvert.DeserializeObject(receivedText);
                HandleData(data);
            }
            catch (IOException)
            {
                Server.DisconnectClient(this);
                return;
            }
            catch (RuntimeBinderException)
            {
                Server.DisconnectClient(this);
                return;
            }
            catch(JsonReaderException) { }

            stream.BeginRead(buffer, 0, buffer.Length, new AsyncCallback(OnRead), null);
        }

        /*
         * This method handles all the incoming data received by the OnRead method.
         */
        private void HandleData(dynamic data)
        {
            //Console.WriteLine(data);
            string tag = data.tag;
            byte[] bytes;
            switch (tag)
            {
                case "chat":
                    bytes = PackageWrapper.SerializeData("chat/receive", new { message = $"{Account.Username}: {data.data.message}" });
                    Server.BroadcastToLobby(Lobby, bytes);
                    break;
                case "login":
                    if(Server.CheckCredentials((string)data.data.username, (string)data.data.password, this))
                        // User credentials matched.
                        SendPacket(PackageWrapper.SerializeData("login/success", new { message = "Successfully logged in." }));
                    else
                        // User credentials do not match with the saved accounts.
                        SendPacket(PackageWrapper.SerializeData("login/error", new { message = "Username and/or password is incorrect." }));
                    break;
                case "register":
                    // Register new account to the server.
                    if(Server.AddAccount((string)data.data.username, (string)data.data.password))
                        // Successfullt created a new account.
                        SendPacket(PackageWrapper.SerializeData("register/success", new { message = "Successfully registered." }));
                    else
                        // Username was not unique.
                        SendPacket(PackageWrapper.SerializeData("register/error", new { message = "Username is not unique" }));
                    break;
                case "create":
                    if (Server.CreateLobby((string)data.data.lobbyName, (string)data.data.gameOwner, (int)data.data.maxPlayers, (MapSize)data.data.mapSize, this))
                        // Successfully created a new lobby.
                        SendPacket(PackageWrapper.SerializeData("create/success", new { message = "Lobby created." }));
                    else
                        // Failed to create a new lobby.
                        SendPacket(PackageWrapper.SerializeData("create/error", new { message = "Unable to make new lobby." }));
                    break;
                case "join":
                    if (Server.JoinLobby((string)data.data.lobbyName, (string)data.data.playerName, this))
                        // Successfully joined the lobby.
                        SendPacket(PackageWrapper.SerializeData("join/success", new { message = "Joined the lobby." }));
                    else
                        // Failed to join the lobby.
                        SendPacket(PackageWrapper.SerializeData("join/error", new { message = "Unable to join this lobby." }));
                    break;
                case "leave":
                    if (Server.LeaveLobby((string)data.data.lobbyName, (string)data.data.playerName, this))
                        // Successfully left the lobby.
                        SendPacket(PackageWrapper.SerializeData("leave/success", new { message = "Left the lobby." }));
                    else
                        // Failed to leave lobby.
                        SendPacket(PackageWrapper.SerializeData("leave/error", new { message = "Unable to leave the lobby." }));
                    break;
                case "refresh/lobby":
                    // Send the current joined lobby to the client
                    SendPacket(PackageWrapper.SerializeData("refresh/lobby/success", new { lobby = Lobby }));
                    break;
                case "refresh":
                    // Start sending the lobby list to the client.
                    Server.StartSendLobbyList(this);
                    break;
                case "refresh/next":
                    // Send a fragment of the lobby list.
                    Server.SendLobbyListFragment(this);
                    break;
                case "game/start":
                    // Start the game of this clients lobby.
                    Server.StartGame(this);
                    break;
                case "game/stop":
                    // Stop the game of this clients lobby.
                    Server.StopGame(this);
                    break;
                case "game/move/response":
                    // Received next move from this client.
                    ReceivedNextMove = true;
                    NextMove = (Direction)data.data.move;
                    break;
                default:
                    Console.WriteLine($"No handling found for tag: {tag}");
                    break;
            }
        }

        /*
         * Helper method for sending packets to the clients.
         */
        public void SendPacket(byte[] bytes)
        {
            stream.Write(bytes, 0, bytes.Length);
        }
    }
}
