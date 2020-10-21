using System;
using System.Collections.Generic;
using System.Text;
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
        // public Game Game { get; set; }
        
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
         * Adds the new player to the lobby if the lobby is not full.
         */
        public bool AddPlayer(string playerName)
        {
            if(Players.Count < MaxPlayers)
            {
                Players.Add(new Player(playerName));
                Console.WriteLine(this);
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
                    Players.Remove(player);
                    Console.WriteLine(this);
                    return true;
                }
            return false;
        }
        public override string ToString()
        {
            string playersString = "";
            foreach(var player in Players)
            {
                playersString += player.Name;
            }
            return $"Lobby name: {Name} Players: {playersString} GameOwner: {GameOwner}";
        }

    }
}
