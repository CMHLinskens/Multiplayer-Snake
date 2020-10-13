using Newtonsoft.Json;
using System;
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

        public NetworkStream GetStream() { return tcpClient.GetStream(); }

        public Client(TcpClient newTcpClient)
        {
            tcpClient = newTcpClient;
            // Start reading from the stream.
            tcpClient.GetStream().BeginRead(buffer, 0, buffer.Length, new AsyncCallback(OnRead), null);
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
            }
            catch (IOException)
            {
                Disconnect();
                return;
            }

            tcpClient.GetStream().BeginRead(buffer, 0, buffer.Length, new AsyncCallback(OnRead), null);
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
                    Program.Broadcast(bytes);
                    break;
                case "login":
                    username = receivedData.data.username;
                    if(Program.CheckCredentials(username, (string)receivedData.data.password))
                        bytes = PackageWrapper.SerializeData("login/success", new { message = "Successfully logged in." });
                    else
                        bytes = PackageWrapper.SerializeData("login/error", new { message = "Username and/or password is incorrect." });
                    tcpClient.GetStream().Write(bytes, 0, bytes.Length);
                    break;
                default:
                    Console.WriteLine($"No handling found for tag: {tag}");
                    break;
            }
        }

        /*
         * This method closes the stream and removes itself from the server.
         */
        private void Disconnect()
        {
            tcpClient.GetStream().Dispose();
            tcpClient.Close();
            Program.Disconnect(this);
        }
    }
}
