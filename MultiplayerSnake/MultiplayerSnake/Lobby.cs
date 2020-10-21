using System;
using System.Collections.Generic;
using System.Text;
using Utils;

namespace MultiplayerSnake
{
    class Lobby
    {
        public string Name { get; set; }
        public List<Player> Players { get; set; }
        public bool IsInGame { get; set; }
        public int MaxPlayers { get; set; }
        public string GameOwner { get; set; }
        public MapSize MapSize { get; set; }
        // public Game Game { get; set; }

        /*
         * Constructor for creating new lobbies.
         */
        public Lobby(string name, string creator, int maxPlayers, MapSize mapSize)
        {
            Name = name;
            Players = new List<Player>();
            Players.Add(new Player(creator));
            IsInGame = false;
            MaxPlayers = maxPlayers;
            GameOwner = creator;
            MapSize = mapSize;
        }

        /*
         * Constructor for parsing data from receiving lobbies on a refresh command.
         */
        public Lobby(string name, List<Player> players, bool isInGame, int maxPlayers, string gameOwner, MapSize mapSize)
        {
            Name = name;
            Players = players;
            IsInGame = isInGame;
            MaxPlayers = maxPlayers;
            GameOwner = gameOwner;
            MapSize = mapSize;
        }

        public override string ToString()
        {
            string playersString = "";
            foreach (var player in Players)
            {
                playersString += player.Name;
            }
            return $"Lobby name: {Name} Players: {playersString} GameOwner: {GameOwner}";
        }
    }
}
