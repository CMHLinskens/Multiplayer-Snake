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
            Lobbies = new ObservableCollection<LobbyViewModel> { new LobbyViewModel(new Lobby("test game", "E-chan", 2, MapSize.size16x16), shellViewModel) };
            this.shellViewModel = shellViewModel;
            Task.Factory.StartNew(RefreshLoopAsync);
            CreateLobbyCommand = new RelayCommand(CreateLobby);
            this.serverConnection = shellViewModel.Program.sc;
        }

        /*
         * Refreshed lobby list every 2 seconds.
         */
        private async Task RefreshLoopAsync()
        {
            while (shellViewModel.Program.sc.LoggedIn)
            {
                // Refresh list
                Lobbies = await Task.Run(() => Refresh());
                // Wait 2 seconds
                Thread.Sleep(2000);
            }
        }

        private async Task<ObservableCollection<LobbyViewModel>> Refresh()
        {
            ObservableCollection<LobbyViewModel> lobbyViewModels = new ObservableCollection<LobbyViewModel>();
            await Task.Run(() => shellViewModel.Program.RefreshLobbyList());
            lobbyViewModels.Clear();
            foreach (Lobby lobby in shellViewModel.Program.sc.Lobbies)
                lobbyViewModels.Add(new LobbyViewModel(lobby, shellViewModel));
            return lobbyViewModels;
        }

        private void CreateLobby()
        {
            SelectedLobbyViewModel = null;
            SelectedViewModel = new CreateLobbyViewModel(shellViewModel);
            CreateButtonVisibility = Visibility.Hidden;
        }
    }
}
