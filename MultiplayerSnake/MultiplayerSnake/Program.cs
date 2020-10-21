using System;
using System.Threading;
using Utils;

namespace MultiplayerSnake
{
    class Program
    {
        static void Main(string[] args)
        {
            ServerConnection sc = new ServerConnection();
            string name = GeneratePlayerNameTest();

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
                    "\n- create" +
                    "\n- join" +
                    "\n- leave" +
                    "\n- refresh");
                input = Console.ReadLine();
                switch (input)
                {
                    case "create":
                        Console.Write("Room name: ");
                        string inputName = Console.ReadLine();
                        Console.Write("Player limit: ");
                        string inputPlayers = Console.ReadLine();
                        sc.CreateLobby(inputName, name, int.Parse(inputPlayers), MapSize.size16x16);
                        break;
                    case "join":
                        Console.Write("Room name: ");
                        input = Console.ReadLine();
                        sc.ConnectToLobby(input, name);
                        break;
                    case "leave":
                        Console.Write("Room name: ");
                        input = Console.ReadLine();
                        sc.LeaveLobby(input, name);
                        break;
                    case "refresh":
                        break;
                    default:
                        Console.WriteLine("Unknown Command\n");
                        break;
                }
            }
        }

        private static string GeneratePlayerNameTest()
        {
            Random random = new Random();
            string name = "";
            for (int i = 0; i < 5; i++)
                name += Convert.ToChar(random.Next(97 ,122));
            return name;
        }
    }
}
