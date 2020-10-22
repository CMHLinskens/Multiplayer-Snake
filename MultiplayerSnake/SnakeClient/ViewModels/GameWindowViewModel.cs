using GalaSoft.MvvmLight.Command;
using SnakeClient.Models;
using SnakeClient.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;

namespace SnakeClient.ViewModels
{
    class GameWindowViewModel : CustomObservableObject
    {
        private Lobby lobby;
        private Player player;
        private ShellViewModel shellViewModel;
        public ObservableCollection<Player> Players{ get; set; }
        public ObservableCollection<string> ChatList { get; set; }
        public string ChatMessage { get; set; }
        public SnakeViewModel SnakeViewModel { get; set; }
        public ICommand StartCommand { get; set; }
        public ICommand QuitCommand { get; set; }
        public GameWindowViewModel(Lobby lobby, ShellViewModel shellViewModel)
        {
            StartCommand = new RelayCommand(Start);
            QuitCommand = new RelayCommand(Quit);
            this.shellViewModel = shellViewModel;
            this.lobby = lobby;
            Players = lobby.Players;

        }

        private void Start()
        {
            SnakeViewModel = new SnakeViewModel();
        }

        private void Quit()
        {

        }
    }
}
