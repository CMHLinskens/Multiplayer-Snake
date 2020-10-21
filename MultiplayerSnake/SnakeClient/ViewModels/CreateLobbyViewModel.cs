using GalaSoft.MvvmLight.Command;
using SnakeClient.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Utils;

namespace SnakeClient.ViewModels
{
    class CreateLobbyViewModel : CustomObservableObject
    {
        private ShellViewModel shellViewModel;
        public string Name { get; set; }
        public ObservableCollection<int> MaxPlayers { get; set; }
        public int SelectedMaxPlayers { get; set; }
        public ObservableCollection<MapSize> MapSizes { get; set; }
        public MapSize SelectedMapSize { get; set; }
        public ICommand CreateCommand { get; set; }
        public CreateLobbyViewModel(ShellViewModel shellViewModel)
        {
            MaxPlayers = new ObservableCollection<int> { 2, 3, 4 };
            MapSizes = new ObservableCollection<MapSize> { MapSize.size16x16, MapSize.size32x32 };
            this.shellViewModel = shellViewModel;

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
            // Create lobby on client
        }
    }
}
