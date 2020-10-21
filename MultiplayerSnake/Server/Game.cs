using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Utils;

namespace Server
{
    class Game
    {
        private Stopwatch watch; // Use this to keep track of execution time of the update method.
        private Timer GameLoop { get; set; }
        public bool IsRunning { get; set; }
        private Lobby Lobby { get; set; }
        private int[][] GameField { get; set; }


        public Game(Lobby lobby)
        {
            Lobby = lobby;
            GameLoop = new Timer(5000);
            GameLoop.Elapsed += UpdateGameAsync;
            GameLoop.AutoReset = true;
            watch = new Stopwatch();
            StartGame();
        }

        /*
         * Updates the game.
         */
        private async void UpdateGameAsync(object sender, ElapsedEventArgs e)
        {
            //watch.Restart();
            //watch.Start();
            List<Direction> nextMoves = await Task.Run(() => Lobby.RequestPlayerData());
            
            // TODO
            // Calculate next output
            // Send ouput to all clients

            //watch.Stop();
            //Console.WriteLine($"Elapsed time {watch.ElapsedMilliseconds}");
        }

        /*
         * Starts the game.
         */
        public void StartGame()
        {
            GameLoop.Start();
            IsRunning = true;
        }

        /*
         * Stops the game.
         */
        public void StopGame()
        {
            GameLoop.Stop();
            IsRunning = false;
        }
    }
}
