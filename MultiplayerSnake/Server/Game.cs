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
        private int gameUpdateSpeed = 500; // time in ms.
        private int mapSize;
        private (int y, int x) food;
        private Random random;
        private Stopwatch watch; // Use this to keep track of execution time of the update method.
        private Timer GameLoop { get; set; }
        public bool IsRunning { get; set; }
        private Lobby Lobby { get; set; }
        private int[,] GameField { get; set; }


        public Game(Lobby lobby)
        {
            Lobby = lobby;
            GameLoop = new Timer(gameUpdateSpeed);
            GameLoop.Elapsed += UpdateGameAsync;
            GameLoop.AutoReset = true;
            random = new Random();
            watch = new Stopwatch();
        }

        /*
         * Updates the game.
         */
        private async void UpdateGameAsync(object sender, ElapsedEventArgs e)
        {
            watch.Restart();
            List<Direction> nextMoves = await Task.Run(() => Lobby.RequestPlayerData());
            
            foreach(var player in Lobby.Players)
                if(player.Alive)
                    MovePlayer(player, nextMoves[Lobby.Players.IndexOf(player)]);

            Console.WriteLine(this);

            // TODO
            // Send ouput to all clients

            watch.Stop();
            GameLoop.Interval = (gameUpdateSpeed - watch.ElapsedMilliseconds);
            //Console.WriteLine($"Elapsed time {watch.ElapsedMilliseconds}");
        }

        public void StartGame()
        {
            Task.Run(() => StartGameAsync()); //  No need to await this task
        }

        /*
         * Starts the game.
         */
        private async Task StartGameAsync()
        {
            await Task.Run(() => InitializeGameField());
            Console.WriteLine(this);
            GameLoop.Start();
            IsRunning = true;
        }

        /*
         * Builds the map according to the given MapSize property.
         */
        private void InitializeGameField()
        {
            mapSize = 0;
            switch (Lobby.MapSize)
            {
                case MapSize.size16x16:
                    mapSize = 16;
                    break;
                case MapSize.size32x32:
                    mapSize = 32;
                    break;
            }

            GameField = new int[mapSize, mapSize];

            for (int y = 0; y < mapSize; y++)
            {
                for (int x = 0; x < mapSize; x++)
                {
                    GameField[y, x] = 0;
                }
            }
            SetStartPositions();
        }

        /*
         * Sets the starting position of every player in the lobby.
         */
        private void SetStartPositions()
        {
            foreach (var player in Lobby.Players)
            {
                player.Alive = true;
                switch (Lobby.Players.IndexOf(player))
                {
                    case 0:
                        player.Position = new List<(int, int)> { (2, 11), (2, 12), (2, 13) };
                        break;
                    case 1:
                        player.Position = new List<(int, int)> { (4, 2), (3, 2), (2, 2) };
                        break;
                    case 2:
                        player.Position = new List<(int, int)> { (13, 4), (13, 3), (13, 2) };
                        break;
                    case 3:
                        player.Position = new List<(int, int)> { (11, 13), (12, 13), (13, 13) };
                        break;
                }
                // Update start position in GameField.
                foreach (var pos in player.Position)
                    GameField[pos.y, pos.x] = Lobby.Players.IndexOf(player) + 2;
            }
            CreateNewFood();
        }

        /*
         * Updates the position of the player by inserting and removing.
         * Updates new position and removes last position in GameField according to the Collision() result.
         */
        private void MovePlayer(Player player, Direction nextMove)
        {
            (int y, int x) newPosition = (0,0);
            switch (nextMove)
            {
                case Direction.up:
                    newPosition = (player.Position[0].y - 1, player.Position[0].x);
                    break;
                case Direction.down:
                    newPosition = (player.Position[0].y + 1, player.Position[0].x);
                    break;
                case Direction.left:
                    newPosition = (player.Position[0].y, player.Position[0].x - 1);
                    break;
                case Direction.right:
                    newPosition = (player.Position[0].y, player.Position[0].x + 1);
                    break;
            }
            newPosition = CheckOutOfBounds(newPosition);
            player.Position.Insert(0, newPosition); // Insert new head position.
            GameField[newPosition.y, newPosition.x] = (Lobby.Players.IndexOf(player) + 2); // Update the new position in the GameField
            CollisionType collisionType;
            if (Collision(out collisionType, newPosition))
            {
                if (collisionType == CollisionType.Apple)
                {
                    player.Length++;
                    CreateNewFood();
                }
                else if (collisionType == CollisionType.Player)
                {
                    // Kill Player
                    player.Position.Clear();
                    player.Alive = false;
                }
            }
            else
            {
                // Remove the last position of player from the GameField.
                GameField[player.Position[player.Position.Count - 1].y, player.Position[player.Position.Count - 1].x] = 0;
                // Remove the last position in the player position list.
                player.Position.RemoveAt(player.Position.Count - 1);
            }
        }

        /*
         * Checks if the new position is out of bounds.
         * If that is the case, loop around.
         */
        private (int, int) CheckOutOfBounds((int y, int x) newPosition)
        {
            if (newPosition.y > mapSize - 1)
                newPosition.y = 0;
            else if (newPosition.y < 0)
                newPosition.y = mapSize - 1;
            else if (newPosition.x > mapSize - 1)
                newPosition.x = 0;
            else if (newPosition.x < 0)
                newPosition.x = mapSize - 1;

            return newPosition;
        }

        /*
         * Places food on a random location that is currently not occupied by a snake. 
         */
        private void CreateNewFood()
        {
            do
            {
                food.y = random.Next(0, mapSize - 1);
                food.x = random.Next(0, mapSize - 1);
            } while (GameField[food.y, food.x] != 0);
            GameField[food.y, food.x] = 1;
        }

        /*
         * Check for collision with food or players.
         */
        private bool Collision(out CollisionType collisionType, (int y, int x) newPosition)
        {
            if(food == newPosition)
            {
                collisionType = CollisionType.Apple;
                return true;
            }
            foreach(var player in Lobby.Players)
            {
                foreach(var pos in player.Position)
                {
                    if(pos == newPosition)
                    {
                        collisionType = CollisionType.Player;
                        return true;
                    }
                }
            }
            collisionType = CollisionType.None;
            return false;
        }

        /*
         * Stops the game.
         */
        public void StopGame()
        {
            GameLoop.Stop();
            IsRunning = false;
        }

        // For testing
        public override string ToString()
        {
            string output = "\n";
            for (int y = 0; y < (GameField.Length / 16); y++)
            {
                output += "{";
                for (int x = 0; x < (GameField.Length / 16); x++)
                {
                    output += GameField[y, x].ToString() + ",";
                }
                output += "},\n";
            }
            output += "\n";
            return output;
        }
    }
}
