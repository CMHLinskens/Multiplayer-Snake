using GalaSoft.MvvmLight.Command;
using SnakeClient.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SnakeClient.ViewModels
{
    class LobbyTabViewModel : CustomObservableObject
    {
        private bool isRefreshing;
        private ShellViewModel shellViewModel;
        public Visibility CreateButtonVisibility { get; set; } = Visibility.Visible;
        public ObservableCollection<LobbyViewModel> Lobbies{ get; set; }
        public ICommand CreateLobbyCommand { get; set; }
        public CustomObservableObject SelectedLobbyViewModel { get; set; }
        public LobbyTabViewModel(ShellViewModel shellViewModel)
        {
            this.shellViewModel = shellViewModel;
            Task.Factory.StartNew(Refresh);
            CreateLobbyCommand = new RelayCommand(CreateLobby);
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
            SelectedLobbyViewModel = new CreateLobbyViewModel();
            CreateButtonVisibility = Visibility.Hidden;
        }
    }
}
