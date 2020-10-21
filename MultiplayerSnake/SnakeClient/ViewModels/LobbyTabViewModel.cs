using GalaSoft.MvvmLight.Command;
using SnakeClient.Models;
using SnakeClient.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Utils;

namespace SnakeClient.ViewModels
{
    class LobbyTabViewModel : CustomObservableObject
    {
        private bool isRefreshing;
        private ShellViewModel shellViewModel;
        private ServerConnection serverConnection;
        private CustomObservableObject _selectedLobbyViewModel;
        public Visibility CreateButtonVisibility { get; set; } = Visibility.Visible;
        public ObservableCollection<LobbyViewModel> Lobbies{ get; set; }
        public ICommand CreateLobbyCommand { get; set; }
        public CustomObservableObject SelectedLobbyViewModel
        {
            get { return _selectedLobbyViewModel; }
            set
            {
                _selectedLobbyViewModel = value;
                SelectedViewModel = value;
                CreateButtonVisibility = Visibility.Visible;
            }
        }
        public CustomObservableObject SelectedViewModel { get; set; }
        public LobbyTabViewModel(ShellViewModel shellViewModel)
        {
            Lobbies = new ObservableCollection<LobbyViewModel> { new LobbyViewModel(new Lobby("test game", "E-chan", 2, MapSize.size16x16)) };
            this.shellViewModel = shellViewModel;
            Task.Factory.StartNew(Refresh);
            CreateLobbyCommand = new RelayCommand(CreateLobby);
            this.serverConnection = shellViewModel.Program.sc;
        }

        private void Refresh()
        {
            while (isRefreshing)
            {
                //Insert code for getting lobbies
                
                Thread.Sleep(500);
            }
        }
        private void CreateLobby()
        {
            SelectedLobbyViewModel = null;
            SelectedViewModel = new CreateLobbyViewModel();
            CreateButtonVisibility = Visibility.Hidden;
        }
    }
}
