using GalaSoft.MvvmLight.Command;
using SnakeClient.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;

namespace SnakeClient.ViewModels
{
    class CreateLobbyViewModel : CustomObservableObject
    {
        public string Name { get; set; }
        public ObservableCollection<int> MaxPlayers { get; set; }
        public int SelectedMaxPlayers { get; set; }
        public ObservableCollection<MapSize> MapSizes { get; set; }
        public MapSize SelectedMapSize { get; set; }
        public ICommand CreateCommand { get; set; }
        public CreateLobbyViewModel()
        {
            MaxPlayers = new ObservableCollection<int> { 2, 3, 4 };
            MapSizes = new ObservableCollection<MapSize> { MapSize.size16x16, MapSize.size32x32 };

            SelectedMaxPlayers = 2;
            SelectedMapSize = MapSize.size16x16;
            CreateCommand = new RelayCommand(CreateGameRequest);
        }

        private void CreateGameRequest()
        {

        }
    }
}
