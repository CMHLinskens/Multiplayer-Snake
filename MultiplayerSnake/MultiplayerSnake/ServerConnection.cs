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

        public ServerConnection()
        {
            tcpClient = new TcpClient();
            Connect(ipAddress, port);
        }

        public ServerConnection(string ipAddress, int port)
        {
            tcpClient = new TcpClient();
            Connect(ipAddress, port);
        }

        private void Connect(string ipAddress, int port)
        {
            try
            {
                tcpClient.Connect(ipAddress, port);

                if (tcpClient.Connected)
                    OnConnected();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                if (!tcpClient.Connected && totalConnectTries < maxReconnectingTries)
                {
                    totalConnectTries++;
                    Connect(ipAddress, port);
                }
                else
                    OnDisconnect();
            }
        }

        private void OnConnected()
        {
            tcpClient.GetStream().BeginRead(buffer, 0, buffer.Length, new AsyncCallback(OnRead), null);
        }

        private void OnDisconnect()
        {
            tcpClient.GetStream().Dispose();
            tcpClient.Close();
        }

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

        private void HandleData(dynamic data)
        {
            Console.WriteLine(data);
        }

        public void SendMessage(string tag, string input)
        {
            byte[] bytes = PackageWrapper.SerializeData(tag, new { message = input });
            tcpClient.GetStream().Write(bytes, 0, bytes.Length);
        }
    }
}
