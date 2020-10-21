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
        public ICommand CreateLobbyCommand { get; set; }
        public CreateLobbyViewModel()
        {
            CreateLobbyCommand = new RelayCommand(RequestLobbyApproval);
        }

        private void RequestLobbyApproval()
        {

        }
    }
}
