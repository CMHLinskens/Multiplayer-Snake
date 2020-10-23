using GalaSoft.MvvmLight.Command;
using SnakeClient.Models;
using SnakeClient.Utils;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Utils;

namespace SnakeClient.ViewModels
{
    class LobbyViewModel : CustomObservableObject, ILobbyViewModel
    {
        private ShellViewModel shellViewModel;
        private LobbyTabViewModel lobbyTabViewModel;
        public Lobby Lobby { get; set; }
        public string Name { get; set; }
        public ObservableCollection<Player> Players { get; set; }
        public bool IsInGame { get; set; }
        public int MaxPlayers { get; set; }
        public string Owner { get; set; }
        public Direction Direction{ get; set; }
        public MapSize MapSize { get; set; }
        public ICommand JoinCommand { get; set; }

        public LobbyViewModel(Lobby lobby, ShellViewModel shellViewModel, LobbyTabViewModel lobbyTabViewModel)
        {
            this.shellViewModel = shellViewModel;
            this.lobbyTabViewModel = lobbyTabViewModel;
            Lobby = lobby;
            Name = Lobby.Name;
            Players = Lobby.Players;
            IsInGame = false;
            MaxPlayers = Lobby.MaxPlayers;
            Owner = Lobby.GameOwner;
            MapSize = (MapSize)Lobby.MapSize;
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
            lobbyTabViewModel.OpenGameWindow(Lobby);
        }

        private void RequestStartGame()
        {

        }
    }
}