using GalaSoft.MvvmLight.Command;
using SnakeClient.Models;
using SnakeClient.Utils;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Utils;

namespace SnakeClient.ViewModels
{
     class LobbyViewModel : CustomObservableObject
    {
        private ShellViewModel shellViewModel;
        private Lobby lobby;
        public string Name { get; set; }
        public ObservableCollection<Player> Players { get; set; }
        public bool IsInGame { get; set; }
        public int MaxPlayers { get; set; }
        public string Owner { get; set; }
        public Direction Direction{ get; set; }
        public MapSize MapSize { get; set; }
        public ICommand JoinCommand { get; set; }
        public LobbyViewModel(string name, int maxPlayers, MapSize mapSize)
        {
            Name = name;
            MaxPlayers = maxPlayers;
            MapSize = mapSize;
        }

        public LobbyViewModel(Lobby lobby, ShellViewModel shellViewModel)
        {
            this.shellViewModel = shellViewModel;
            this.lobby = lobby;
            Name = this.lobby.Name;
            Players = lobby.Players;
            IsInGame = false;
            MaxPlayers = this.lobby.MaxPlayers;
            Owner = this.lobby.GameOwner;
            MapSize = (MapSize)this.lobby.MapSize;
            JoinCommand = new RelayCommand(async () => await JoinLobbyAsync());
        }

        private async Task JoinLobbyAsync()
        {
            if (await Task.Run(() => shellViewModel.Program.JoinLobby(Name, shellViewModel.Name)))
                LobbyJoined();
            else
                JoinLobbyFailed();
        }

        private void JoinLobbyFailed()
        {
        }

        private void LobbyJoined()
        {
            // Join lobby on client
        }

        private void RequestStartGame()
        {

        }
    }
}