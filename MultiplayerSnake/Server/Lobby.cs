using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Utils;

namespace Server
{
    class Lobby
    {
        public string Name { get; set; }
        public List<Player> Players { get; set; }
        public bool IsInGame { get; set; }
        public int MaxPlayers { get; set; }
        public string GameOwner { get; set; }
        public MapSize MapSize { get; set; }
        [JsonIgnore]
        public Game Game { get; set; }
        
        public Lobby(string name, string creator, int maxPlayers, MapSize mapSize)
        {
            Name = name;
            Players = new List<Player>();
            Players.Add(new Player(creator));
            IsInGame = false;
            MaxPlayers = maxPlayers;
            GameOwner = creator;
            MapSize = mapSize;
            Game = new Game(this);
        }

        /*
         * Adds the new player to the lobby if the lobby is not full.
         */
        public bool AddPlayer(string playerName)
        {
            // Check if max player count has been reached.
            if(Players.Count < MaxPlayers)
            {
                // Check if someone with the same name is already in this lobby.
                foreach(var player in Players)
                    if (player.Name == playerName)
                        return false;

                Players.Add(new Player(playerName));
                Console.WriteLine(this);
                StartGame();
                return true;
            }
            return false;
        }

        /*
         * Removes the player from the lobby if the player is currently in this lobby.
         */
        internal bool RemovePlayer(string playerName)
        {
            foreach (var player in Players)
                if (player.Name == playerName)
                {
                    if (IsInGame)
                        Game.KillPlayer(player);
                    Players.Remove(player);
                    Console.WriteLine(this);
                    return true;
                }
            return false;
        }

        /*
         * Asks all players in this lobby to send their data.
         */
        public async Task<List<Direction>> RequestPlayerData()
        {
            // Asks all the clients for their next move.
            foreach (var player in Players)
            {
                Server.GetClientWithUserName(player.Name).ReceivedNextMove = false;
                Server.GetClientWithUserName(player.Name).SendPacket(PackageWrapper.SerializeData("game/move/request", new { }));
            }

            List<Direction> nextMoves = new List<Direction>();

            // Wait for the data.
            await Task.Run(() =>
            {
                foreach (var player in Players)
                {
                    while (!Server.GetClientWithUserName(player.Name).ReceivedNextMove) 
                        Thread.Sleep(5);
                    nextMoves.Add(Server.GetClientWithUserName(player.Name).NextMove);
                }
            });

            return nextMoves;
        }

        /*
         * Start the game and notify all players in lobby.
         */
        public void StartGame()
        {
            IsInGame = true;
            Game.StartGame();
            foreach (var player in Players)
                Server.GetClientWithUserName(player.Name).SendPacket(PackageWrapper.SerializeData("game/start/success", new { }));
        }

        /*
         * Method used for testing output.
         */
        public override string ToString()
        {
            string playersString = "";
            foreach(var player in Players)
            {
                playersString += player.Name;
            }
            return $"Lobby name: {Name} Players: {playersString} GameOwner: {GameOwner}";
        }

        /*
         * Stop the game and delete it.
         */
        internal void DeleteGame()
        {
            Game.StopGame();
            Game = null;
        }
    }
}
