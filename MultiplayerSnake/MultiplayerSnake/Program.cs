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
            int count = 0;
            while (true)
            {
                Console.WriteLine("Commands:" +
                    "\n-create" +
                    "\n-join");
                input = Console.ReadLine();
                switch (input)
                {
                    case "create":
                        sc.CreateLobby("testroom" + count++, "testname" + count++);
                        break;
                    case "join":
                        Console.Write("Room name: ");
                        input = Console.ReadLine();
                        sc.ConnectToLobby(input, "testname" + count++);
                        break;
                    case "refresh":
                        //sc.RefreshLobbyList();
                        break;
                    case "refresh/fragment":
                        break;
                    default:
                        Console.WriteLine("Unknown Command\n");
                        break;
                }
            }
        }
    }
}
