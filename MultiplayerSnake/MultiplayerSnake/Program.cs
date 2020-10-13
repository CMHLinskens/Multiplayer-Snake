using System;

namespace MultiplayerSnake
{
    class Program
    {
        static void Main(string[] args)
        {
            ServerConnection sc = new ServerConnection();

            string input = "";
            while (true)
            {
                input = Console.ReadLine();
                sc.SendMessage("chat", input);
            }
        }
    }
}
