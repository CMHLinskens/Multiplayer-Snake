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
            clients = new List<Client>();
            listener = new TcpListener(IPAddress.Any, 1330);
            listener.Start();
            listener.BeginAcceptTcpClient(new AsyncCallback(Connect), null);
            Console.ReadLine();
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

        internal static void Broadcast(byte[] bytes)
        {
            foreach (var client in clients)
                client.GetStream().Write(bytes, 0, bytes.Length);
        }
    }
}
