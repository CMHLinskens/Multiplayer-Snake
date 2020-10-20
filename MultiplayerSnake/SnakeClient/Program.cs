using System;
using System.Threading;

namespace SnakeClient
{
    class Program
    {
        public ServerConnection sc { get; }

        public Program()
        {
            sc = new ServerConnection();
        }

        public bool Login(string username, string password)
        {
            sc.Login(username, password);

            // Wait for the sc to connect
            while (!sc.HasReceivedLoginMessage())
            {
                Thread.Sleep(10);
            }

            return sc.IsLoggedIn();
        }
    }
}
