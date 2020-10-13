using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Mail;
using System.Net.Sockets;
using System.Text;
using Utils;

namespace Server
{
    class Client
    {
        private TcpClient tcpClient;
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
                    bytes = PackageWrapper.SerializeData(tag, receivedData.data);
                    Program.Broadcast(bytes);
                    break;
                default:
                    Console.WriteLine($"No handling found for command: {tag}");
                    break;
            }
        }

        /*
         * This method closes the stream and removes itself from the server.
         */
        private void Disconnect()
        {
            tcpClient.GetStream().Close();
            Program.Disconnect(this);
        }
    }
}
