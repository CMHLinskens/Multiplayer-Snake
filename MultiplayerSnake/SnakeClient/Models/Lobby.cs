using SnakeClient.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Utils;

namespace SnakeClient.Models
{
    class Lobby : CustomObservableObject
    {
        public string Name { get; set; }
        public ObservableCollection<Player> Players { get; set; }
        public bool IsInGame { get; set; }
        public int MaxPlayers { get; set; }
        public string GameOwner { get; set; }
        public MapSize MapSize { get; set; }

        public Lobby(string name, string creator, int maxPlayers, MapSize mapSize)
        {
            Name = name;
            Players = new ObservableCollection<Player>();
            Players.Add(new Player(creator));
            IsInGame = false;
            MaxPlayers = maxPlayers;
            GameOwner = creator;
            MapSize = mapSize;
        }

        /*
         * Constructor for parsing data from received lobbies from refresh command.
         */
        public Lobby(string name, ObservableCollection<Player> players, bool isInGame, int maxPlayers, string gameOwner, MapSize mapSize)
        {
            Name = name;
            Players = players;
            IsInGame = isInGame;
            MaxPlayers = maxPlayers;
            GameOwner = gameOwner;
            MapSize = mapSize;
        }

        public Player FindPlayerByName(string playerName)
        {
            foreach(var player in Players)
                if (player.Name == playerName)
                    return player;

            return null;
        }
    }
}
