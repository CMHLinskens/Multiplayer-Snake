using Microsoft.CSharp.RuntimeBinder;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using Utils;

namespace Server
{
    class Client
    {
        private TcpClient tcpClient;
        private string username;
        private byte[] buffer = new byte[1024];
        public List<Lobby> tempRefreshLobbyList { get; set; } // Use this list to temporary store all lobbies to send them in packets to client.
        public NetworkStream stream { get; }

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
                dynamic receivedData = JsonConvert.DeserializeObject(receivedText);
                HandleData(receivedData);
            }
            catch (IOException)
            {
                Server.Disconnect(this);
                return;
            }
            catch (RuntimeBinderException)
            {
                Server.Disconnect(this);
                return;
            }

            stream.BeginRead(buffer, 0, buffer.Length, new AsyncCallback(OnRead), null);
        }

        /*
         * This method handles all the incoming data received by the OnRead method.
         */
        private void HandleData(dynamic receivedData)
        {
            Console.WriteLine(receivedData);
            string tag = receivedData.tag;
            byte[] bytes;
            switch (tag)
            {
                case "chat":
                    bytes = PackageWrapper.SerializeData(tag, new { message = $"{username}: {receivedData.data.message}" });
                    Server.Broadcast(bytes);
                    break;
                case "login":
                    username = receivedData.data.username;
                    if(Server.CheckCredentials(username, (string)receivedData.data.password))
                        // User credentials matched.
                        SendPacket(PackageWrapper.SerializeData("login/success", new { message = "Successfully logged in." }));
                    else
                        // User credentials do not match with the saved accounts.
                        SendPacket(PackageWrapper.SerializeData("login/error", new { message = "Username and/or password is incorrect." }));
                    break;
                case "register":
                    // Register new account to the server.
                    Server.AddAccount((string)receivedData.data.username, (string)receivedData.data.password);
                    break;
                case "create":
                    if (Server.CreateLobby((string)receivedData.data.lobbyName, (string)receivedData.data.gameOwner))
                        // Successfully created a new lobby.
                        SendPacket(PackageWrapper.SerializeData("create/success", new { message = "Lobby created." }));
                    else
                        // Failed to create a new lobby.
                        SendPacket(PackageWrapper.SerializeData("create/error", new { message = "Unable to make new lobby." }));
                    break;
                case "join":
                    if (Server.JoinLobby((string)receivedData.data.lobbyName, (string)receivedData.data.playerName))
                        // Successfully joined the lobby.
                        SendPacket(PackageWrapper.SerializeData("join/success", new { message = "Joined the lobby." }));
                    else
                        // Failed to join the lobby.
                        SendPacket(PackageWrapper.SerializeData("join/error", new { message = "Unable to join this lobby." }));
                    break;
                case "refresh":
                    Server.StartSendLobbyList(this);
                    break;
                case "refresh/next":
                    Server.SendLobbyListFragment(this);
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
