using System;
using System.Threading;

namespace MultiplayerSnake
{
    public class Program
    {
        public Program()
        {

        }
        public bool Login(string username, string password)
        {
            ServerConnection sc = new ServerConnection();

            sc.Login(username, password);

            // Wait for the sc to connect
            while (!sc.HasReceivedLoginMessage())
            {
                Thread.Sleep(10);
            }

            return sc.IsLoggedIn();

            //string input = "";
            //while (true)
            //{
            //    input = Console.ReadLine();
            //    sc.SendChat(input);
            //}
        }
    }
}
