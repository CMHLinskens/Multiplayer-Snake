using SnakeClient.Models;
using SnakeClient.Utils;
using System.Collections.ObjectModel;
using Utils;

namespace SnakeClient.ViewModels
{
     class LobbyViewModel : CustomObservableObject
    {
        private Lobby lobby;
        public string Name { get; set; }
        public ObservableCollection<Player> Players { get; set; }
        public bool IsInGame { get; set; }
        public int MaxPlayers { get; set; }
        public string Owner { get; set; }
        public Direction Direction{ get; set; }
        public MapSize MapSize { get; set; }
        public LobbyViewModel(string name, int maxPlayers, MapSize mapSize)
        {
            Name = name;
            MaxPlayers = maxPlayers;
            MapSize = mapSize;
        }

        public LobbyViewModel(Lobby lobby)
        {
            this.lobby = lobby;
            Name = this.lobby.Name;
            Players = new ObservableCollection<Player>();
            Players.Add(new Player(this.lobby.GameOwner));
            IsInGame = false;
            MaxPlayers = this.lobby.MaxPlayers;
            Owner = this.lobby.GameOwner;
            MapSize = (MapSize)this.lobby.MapSize;
        }

        private void RequestStartGame()
        {

        }
    }
}