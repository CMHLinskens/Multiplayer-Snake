using GalaSoft.MvvmLight.Command;
using SnakeClient.Models;
using SnakeClient.Utils;
using SnakeClient.Views;
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
        private string selectedLobbyName;
        private CustomObservableObject _selectedLobbyViewModel;
        public Visibility CreateButtonVisibility { get; set; } = Visibility.Visible;
        public ObservableCollection<LobbyViewModel> Lobbies{ get; set; }
        public ICommand CreateLobbyCommand { get; set; }
        public CustomObservableObject SelectedLobbyViewModel
        {
            get { return _selectedLobbyViewModel; }
            set
            {
                LobbyViewModel lobbyValue = value as LobbyViewModel;
                if(lobbyValue != null)
                    selectedLobbyName = lobbyValue.Name;
                
                _selectedLobbyViewModel = value;
                SelectedViewModel = value;
                CreateButtonVisibility = Visibility.Visible;
            }
        }
        public CustomObservableObject SelectedViewModel { get; set; }
        public LobbyTabViewModel(ShellViewModel shellViewModel)
        {
            selectedLobbyName = "";
            Lobbies = new ObservableCollection<LobbyViewModel> { new LobbyViewModel(new Lobby("TestLobby", "Test", 2, MapSize.size16x16), shellViewModel, this)};
            this.shellViewModel = shellViewModel;
            Task.Factory.StartNew(RefreshLoopAsync);
            CreateLobbyCommand = new RelayCommand(CreateLobby);
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

        /*
         * Add the new retrieved lobbies to the GUI.
         */
        private async Task<ObservableCollection<LobbyViewModel>> Refresh()
        {
            ObservableCollection<LobbyViewModel> lobbyViewModels = new ObservableCollection<LobbyViewModel>();
            await Task.Run(() => shellViewModel.Program.RefreshLobbyList());
            foreach (Lobby lobby in shellViewModel.Program.sc.Lobbies)
            {
                lobbyViewModels.Add(new LobbyViewModel(lobby, shellViewModel, this));
                if (lobby.Name == selectedLobbyName)
                    SelectedLobbyViewModel = lobbyViewModels[lobbyViewModels.Count - 1];
            }
            return lobbyViewModels;
        }

        private void CreateLobby()
        {
            SelectedLobbyViewModel = null;
            selectedLobbyName = "";
            SelectedViewModel = new CreateLobbyViewModel(shellViewModel, this);
            CreateButtonVisibility = Visibility.Hidden;
        }
        /*
         * Opens new window where the game takes place
         */
        public void OpenGameWindow(Lobby lobby)
        {
            shellViewModel.Visibility = Visibility.Hidden;
            GameWindow gameWindow = new GameWindow();
            gameWindow.DataContext = new GameWindowViewModel(lobby, this.shellViewModel);
            gameWindow.Show();
        }
    }
}
