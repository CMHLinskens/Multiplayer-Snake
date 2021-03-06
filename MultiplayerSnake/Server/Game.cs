﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Timers;
using Utils;

namespace Server
{
    public class Game
    {
        private int gameUpdateSpeed = 250; // time in ms.
        private int mapSize;
        private (int y, int x) food;
        private int foodWinCondition = 20;
        private int playerOffset = 2;
        private int headOffset = 4;
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
            // Get next moves from the clients.
            List<Direction> nextMoves = await Task.Run(() => Lobby.RequestPlayerData());
            
            // Process all moves and update the game.
            foreach(var player in Lobby.Players)
                if(player.Alive)
                    MovePlayer(player, nextMoves[Lobby.Players.IndexOf(player)]);

            // Check if snakes are colliding with each other
            foreach (var player in Lobby.Players)
                if (player.Alive)
                    CheckCollisionWithSnakes(player);

            // Send new update to all clients in this lobby.
            foreach (var player in Lobby.Players)
                Server.GetClientWithUserName(player.Name).SendPacket(PackageWrapper.SerializeData("game/update", new { gameField = GameField }));

            watch.Stop();
            GameLoop.Interval = (gameUpdateSpeed - watch.ElapsedMilliseconds);
        }

        public void StartGame()
        {
            Task.Run(() => StartGameAsync());
        }

        /*
         * Starts the game.
         */
        private async Task StartGameAsync()
        {
            if(IsRunning) { return; }
            Lobby.IsInGame = true;
            IsRunning = true;
            await Task.Run(() => InitializeGameField());
            GameLoop.Start();
        }

        /*
         * Builds the map according to the given MapSize property.
         */
        public void InitializeGameField()
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
                player.Length = 3;
                switch (Lobby.Players.IndexOf(player))
                {
                    case 0:
                        player.Position = new List<(int, int)> { (2, 11), (2, 12), (2, 13) };
                        break;
                    case 1:
                        player.Position = new List<(int, int)> { (4, 2), (3, 2), (2, 2) };
                        break;
                    case 2:
                        player.Position = new List<(int, int)> { (mapSize - 3, 4), (mapSize - 3, 3), (mapSize - 3, 2) };
                        break;
                    case 3:
                        player.Position = new List<(int, int)> { (11, mapSize - 3), (12, mapSize - 3), (13, mapSize - 3) };
                        break;
                }
                // Update start position in GameField.
                for (int i = 0; i < player.Position.Count; i++)
                {
                    if (i == 0) GameField[player.Position[0].y, player.Position[0].x] = Lobby.Players.IndexOf(player) + playerOffset + headOffset; // Set head pos
                    else GameField[player.Position[i].y, player.Position[i].x] = Lobby.Players.IndexOf(player) + playerOffset; // Set body positions
                }
            }
            CreateNewFood();
        }

        public void SetFoodPos(int x, int y)
        {
            food.x = x;
            food.y = y;
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
            if (GameField[newPosition.y, newPosition.x]  < 2)
                GameField[newPosition.y, newPosition.x] = (Lobby.Players.IndexOf(player) + playerOffset + headOffset); // Update the new position in the GameField.
            GameField[player.Position[1].y, player.Position[1].x] = (Lobby.Players.IndexOf(player) + playerOffset); // Set the previous head location to a body number.
            if (CollisionWithFood(newPosition))
            {
                player.Length++;
                CreateNewFood();
                CheckForWinByFood(player);
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
         * Checks if the player is on the food tile and should grow.
         */
        public bool CollisionWithFood((int y, int x) newPosition)
        {
            return food == newPosition;
        }

        /*
         * Checks if the players head is colliding with other snakes.
         */
        private void CheckCollisionWithSnakes(Player player)
        {
            foreach (var p in Lobby.Players)
            {
                if (p.Alive)
                {
                    lock (p.Position)
                    {
                        foreach (var pos in p.Position)
                        {
                            if (pos == player.Position[0])
                            {
                                if (GameField[pos.y, pos.x] != (Lobby.Players.IndexOf(player) + playerOffset + headOffset))
                                {
                                    //int snakeNr = (Lobby.Players.IndexOf(player) + playerOffset + headOffset);
                                    KillPlayer(player);
                                    //if(GameField[pos.y, pos.x] != snakeNr)
                                        // Restore the square that has been hit on the snake.
                                    GameField[pos.y, pos.x] = Lobby.Players.IndexOf(p) + playerOffset;
                                    return;
                                }
                            }
                        }
                    }
                }
            }

        }

        /*
         * Check if the game has been won by food consumption.
         */
        private void CheckForWinByFood(Player player)
        {
            if (player.Length >= foodWinCondition)
            {
                EndGame(player);
            }
        }
        
        /*
         * Check if the game has been won by elimination.
         */
        private void CheckForWinByElimination()
        {
            Player lastPlayer = null;
            foreach (var player in Lobby.Players)
            {
                if (player.Alive)
                {
                    if (lastPlayer == null)
                        lastPlayer = player;
                    else
                        return;
                }
            }
            EndGame(lastPlayer);
        }

        private void EndGame(Player player)
        {
            StopGame();

            foreach (var p in Lobby.Players)
                Server.GetClientWithUserName(p.Name).SendPacket(PackageWrapper.SerializeData("game/end", new { playerName = player.Name }));
        }

        /*
         * Checks if the new position is out of bounds.
         * If that is the case, loop around.
         */
        public (int, int) CheckOutOfBounds((int y, int x) newPosition)
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
         * Disables the player and removes all positions in the GameField
         */
        public void KillPlayer(Player player)
        {
            player.Alive = false;
            // Remove all positions in the GameField
            foreach (var pos in player.Position)
                GameField[pos.y, pos.x] = 0;
            
            player.Position.Clear();

            CheckForWinByElimination();
        }

        /*
         * Stops the game.
         */
        public void StopGame()
        {
            GameLoop.Stop();
            Lobby.IsInGame = false;
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
