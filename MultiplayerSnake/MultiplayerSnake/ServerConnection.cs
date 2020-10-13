using Microsoft.VisualBasic.CompilerServices;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using Utils;

namespace MultiplayerSnake
{
    class ServerConnection
    {
        private TcpClient tcpClient;
        private string ipAddress = "127.0.0.1";
        private int port = 1330;
        private int totalConnectTries;
        private readonly int maxReconnectingTries = 3;
        private byte[] buffer = new byte[1024];
        private string username = "default";
        private bool receivedLoginMessage = false;
        private bool loggedIn = false;

        public bool HasReceivedLoginMessage() { return receivedLoginMessage; }
        public bool IsLoggedIn() { return loggedIn; }

        public ServerConnection()
        {
            tcpClient = new TcpClient();
            Connect(ipAddress, port);
        }

        /*
         * Connects the client to the server.
         * If unsuccessful try again for 3 times.
         */
        private void Connect(string ipAddress, int port)
        {
            try
            {
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
                    OnDisconnect();
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
        private void OnDisconnect()
        {
            tcpClient.GetStream().Dispose();
            tcpClient.Close();
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
                OnDisconnect();
                return;
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
                    loggedIn = true;
                    receivedLoginMessage = true;
                    Console.WriteLine($"{data.data.message}");
                    break;
                case "login/error":
                    receivedLoginMessage = true;
                    Console.WriteLine($"{data.data.message}");
                    break;
                default:
                    Console.WriteLine($"No handling found for tag: {tag}");
                    break;
            }
        }

        #region // Writer functions
        /*
         * Send chat message to server.
         */
        public void SendChat(string input)
        {
            byte[] bytes = PackageWrapper.SerializeData("chat", new { message = input });
            tcpClient.GetStream().Write(bytes, 0, bytes.Length);
        }
        /*
         * Sends the user credentials to the server to login.
         */
        public void Login(string username, string password)
        {
            receivedLoginMessage = false;
            byte[] bytes = PackageWrapper.SerializeData("login", new { username = username, password = password });
            tcpClient.GetStream().Write(bytes, 0, bytes.Length);
        }
        /*
         * Connects the client to the lobby with the same name.
         */
        public void ConnectToLobby(string lobbyName, string password)
        {
            byte[] bytes = PackageWrapper.SerializeData("join", new { lobbyName = lobbyName, password = password });
            tcpClient.GetStream().Write(bytes, 0, bytes.Length);
        }
        #endregion
    }
}
