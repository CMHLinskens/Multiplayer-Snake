using GalaSoft.MvvmLight.Command;
using SnakeClient.Models;
using SnakeClient.Utils;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Utils;

namespace SnakeClient.ViewModels
{
    class CreateLobbyViewModel : CustomObservableObject, ILobbyViewModel
    {
        private ShellViewModel shellViewModel;
        private readonly LobbyTabViewModel lobbyTabViewModel;

        public string Name { get; set; }
        public ObservableCollection<int> MaxPlayers { get; set; }
        public int SelectedMaxPlayers { get; set; }
        public ObservableCollection<MapSize> MapSizes { get; set; }
        public MapSize SelectedMapSize { get; set; }
        public ICommand CreateCommand { get; set; }
        public Lobby Lobby { get; set; }

        public CreateLobbyViewModel(ShellViewModel shellViewModel, LobbyTabViewModel lobbyTabViewModel)
        {
            MaxPlayers = new ObservableCollection<int> { 2, 3, 4 };
            MapSizes = new ObservableCollection<MapSize> { MapSize.size16x16, MapSize.size32x32 };
            this.shellViewModel = shellViewModel;
            this.lobbyTabViewModel = lobbyTabViewModel;
            SelectedMaxPlayers = 2;
            SelectedMapSize = MapSize.size16x16;
            CreateCommand = new RelayCommand(async () => await CreateGameRequestAsync());
        }

        private void CreateGameRequest()
        {
            shellViewModel.Program.CreateLobby(Name, shellViewModel.Name, SelectedMaxPlayers, SelectedMapSize);
        }

        private async Task CreateGameRequestAsync()
        {
            Lobby = new Lobby(Name, shellViewModel.Name, SelectedMaxPlayers, SelectedMapSize);
            if (await Task.Run(() => shellViewModel.Program.CreateLobby(Name, shellViewModel.Name, SelectedMaxPlayers, SelectedMapSize)))
                LobbyCreated();
            else
                CreateLobbyFailed();

        }

        private void CreateLobbyFailed()
        {
        }

        private void LobbyCreated()
        {
            this.lobbyTabViewModel.OpenGameWindow(Lobby);
            // Create lobby on client
        }
    }
}
