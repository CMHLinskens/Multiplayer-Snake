using GalaSoft.MvvmLight.Command;
using SnakeClient.Models;
using SnakeClient.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Utils;

namespace SnakeClient.ViewModels
{
    class GameWindowViewModel : CustomObservableObject, INotifyPropertyChanged
    {
        private Lobby _lobby;
        private Player player;
        private ShellViewModel shellViewModel;

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public ObservableCollection<Player> Players{ get; set; }
        //public ObservableCollection<string> ChatList { get; set; }

        public ObservableCollection<string> ChatList { get; set; }

        public string ChatMessage { get; set; }
        public SnakeViewModel SnakeViewModel { get; set; }
        public Visibility StartButtonVisibility { get; set; }
        public Lobby Lobby { 
            get 
            { 
                return _lobby; 
            } 
            set
            {
                _lobby = value;
                player = value.FindPlayerByName(shellViewModel.Name);
                if (player != null)
                {
                    if (!this.player.Name.Equals(this.Lobby.GameOwner))
                        StartButtonVisibility = Visibility.Hidden;
                    else
                        StartButtonVisibility = Visibility.Visible;
                }

            }
        }
        public ICommand StartCommand { get; set; }
        public ICommand QuitCommand { get; set; }
        public ICommand KeyDownCommand { get; set; }
        public ICommand KeyLeftCommand { get; set; }
        public ICommand KeyUpCommand { get; set; }
        public ICommand KeyRightCommand { get; set; }
        public ICommand KeyEnterCommand { get; set; }
        public GameWindowViewModel(Lobby lobby, ShellViewModel shellViewModel)
        {
            ChatList = new ObservableCollection<string>();
            StartCommand = new RelayCommand(RequestStartGame);
            QuitCommand = new RelayCommand<ICloseable>(Quit);
            KeyEnterCommand = new RelayCommand(SendMessage);
            shellViewModel.Program.sc.GameField = new int[16, 16];
            this.shellViewModel = shellViewModel;
            this.Lobby = lobby;
            Players = lobby.Players;
            Task.Factory.StartNew(RefreshLoopAsync);
            Task.Factory.StartNew(ChatLoopAsync);

            BindKeys();
            Task.Factory.StartNew(WaitForGameStart);

        }

        private void SendMessage()
        {
            shellViewModel.Program.sc.SendChat(ChatMessage);
            ChatMessage = "";
        }

        private void WaitForGameStart()
        {
            while (!shellViewModel.Program.sc.ReceivedGameStartMessage)
            {
                Thread.Sleep(10);
            }
            shellViewModel.Program.sc.ReceivedGameStartMessage = false;
            Start();
            Task.Factory.StartNew(WaitForGameEnd);
        }

        private void WaitForGameEnd()
        {
            while (!shellViewModel.Program.sc.ReceivedGameFinishedMessage)
            {
                Thread.Sleep(10);
            }
            shellViewModel.Program.sc.ReceivedGameFinishedMessage = false;
            WaitForGameStart();
        }

        private void EndGame()
        {
            // iets
        }

        /*
         * Refreshes this lobby every 1000 ms.
         */
        private async Task RefreshLoopAsync()
        {
            while (shellViewModel.Program.sc.LoggedIn)
            {
                // Refresh list
                Lobby = await Task.Run(() => Refresh());
                Players = Lobby.Players;
                player = Lobby.FindPlayerByName(shellViewModel.Name);

                Thread.Sleep(1000);
            }
        }

        /*
         * Checks if a new chat message has been received.
         */
        private async Task ChatLoopAsync()
        {
            while (shellViewModel.Program.sc.LoggedIn)            {
                string newChat = await Task.Run(() => shellViewModel.Program.ChatRefresh());
                App.Current.Dispatcher.Invoke(delegate { ChatList.Add(newChat); }); // Make the collection notify by using the ui thread
            }
        }

        /*
         * Sends refresh command to server and waits for a response.
         */
        private async Task<Lobby> Refresh()
        {
            await Task.Run(() => shellViewModel.Program.RefreshSpecificLobby());
            return shellViewModel.Program.sc.joinedLobby;
        }

        private void BindKeys()
        {
            KeyUpCommand = new RelayCommand(() => { if(shellViewModel.Program.sc.MoveDirection != Direction.down) SetMoveDirection(Direction.up); });
            KeyDownCommand = new RelayCommand(() => { if (shellViewModel.Program.sc.MoveDirection != Direction.up) SetMoveDirection(Direction.down); });
            KeyLeftCommand = new RelayCommand(() => { if (shellViewModel.Program.sc.MoveDirection != Direction.right) SetMoveDirection(Direction.left); });
            KeyRightCommand = new RelayCommand(() => { if (shellViewModel.Program.sc.MoveDirection != Direction.left) SetMoveDirection(Direction.right); });
        }

        private void RequestStartGame()
        {
            shellViewModel.Program.StartGame();
        }

        private void Start()
        {
            GetStartDirection();
            SnakeViewModel = new SnakeViewModel(this.shellViewModel);
            Lobby.IsInGame = true;
            Task.Factory.StartNew(DrawLoopAsync);
        }

        /*
         * Determine the right start direction for this player.
         */
        private void GetStartDirection()
        {
            switch (Lobby.Players.IndexOf(player))
            {
                case 0:
                    shellViewModel.Program.sc.MoveDirection = Direction.left;
                    break;
                case 1:
                    shellViewModel.Program.sc.MoveDirection = Direction.down;
                    break;
                case 2:
                    shellViewModel.Program.sc.MoveDirection = Direction.right;
                    break;
                case 3:
                    shellViewModel.Program.sc.MoveDirection = Direction.up;
                    break;
            }
        }

        private async void Quit(ICloseable window)
        {
            if (await Task.Run(() => shellViewModel.Program.LeaveLobby(Lobby.Name, shellViewModel.Name)))
                LeftLobby();
            else
                FailedToLeftLobby();
            if(window != null)
                window.Close();
            shellViewModel.Visibility = Visibility.Visible;
        }

        private void LeftLobby()
        { 
            // TODO
            // Quit this window and return to client view.
            
        }
        private void FailedToLeftLobby() { }

        private void SetMoveDirection(Direction newDirection)
        {
            shellViewModel.Program.sc.MoveDirection = newDirection;
        }

        private async Task DrawLoopAsync()
        {
            while (Lobby.IsInGame)
            {
                if (shellViewModel.Program.sc.ReceivedNewUpdate)
                {
                    await Task.Run(() => SnakeViewModel.Draw());
                    shellViewModel.Program.sc.ReceivedNewUpdate = false;
                }
                Thread.Sleep(10);
            }
        }
    }
}
