using System;
using System.Threading;

namespace MultiplayerSnake
{
    class Program
    {
        static void Main(string[] args)
        {
            ServerConnection sc = new ServerConnection();

            while (!sc.IsLoggedIn())
            {
                Console.Write("Username: ");
                string username = Console.ReadLine();
                Console.Write("Password: ");
                string password = Console.ReadLine();

                sc.Login(username, password);

                // Wait for the sc to connect
                while (!sc.HasReceivedLoginMessage())
                    Thread.Sleep(10);
            }

            string input = "";
            while (true)
            {
                input = Console.ReadLine();
                sc.SendChat(input);
            }
        }
    }
}
