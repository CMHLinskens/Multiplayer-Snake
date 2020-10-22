using GalaSoft.MvvmLight.Command;
using SnakeClient.Models;
using SnakeClient.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using Utils;

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
        public ICommand KeyDownCommand { get; set; }
        public ICommand KeyLeftCommand { get; set; }
        public ICommand KeyUpCommand { get; set; }
        public ICommand KeyRightCommand { get; set; }
        public GameWindowViewModel(Lobby lobby, ShellViewModel shellViewModel)
        {
            StartCommand = new RelayCommand(Start);
            QuitCommand = new RelayCommand(async () => await Quit());
            shellViewModel.Program.sc.GameField = new int[16, 16];
            this.shellViewModel = shellViewModel;
            this.lobby = lobby;
            Players = lobby.Players;
            Task.Factory.StartNew(RefreshLoopAsync);

            BindKeys();
            //Task.Factory.StartNew(WaitForGameStart);
        }

        private void WaitForGameStart()
        {
            while (!shellViewModel.Program.sc.ReceivedGameStartMessage)
            {
                Thread.Sleep(10);
            }
            shellViewModel.Program.sc.ReceivedGameStartMessage = false;
            Start();
        }

        /*
         * Refreshes this lobby every 1000 ms.
         */
        private async Task RefreshLoopAsync()
        {
            while (shellViewModel.Program.sc.LoggedIn)
            {
                // Refresh list
                lobby = await Task.Run(() => Refresh());
                Players = lobby.Players;
                //if(lobby.IsInGame)
                //    if(SnakeViewModel == null) { Start(); }
                // Wait 1000 ms
                Thread.Sleep(1000);
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
            KeyUpCommand = new RelayCommand(() => SetMoveDirection(Direction.up));
            KeyDownCommand = new RelayCommand(() => SetMoveDirection(Direction.down));
            KeyLeftCommand = new RelayCommand(() => SetMoveDirection(Direction.left));
            KeyRightCommand = new RelayCommand(() => SetMoveDirection(Direction.right));
        }
        private void Start()
        {
            SnakeViewModel = new SnakeViewModel(this.shellViewModel);
            Task.Factory.StartNew(DrawLoopAsync);
        }

        private async Task Quit()
        {
            if (await Task.Run(() => shellViewModel.Program.LeaveLobby(lobby.Name, shellViewModel.Name)))
                LeftLobby();
            else
                FailedToLeftLobby();
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
            while (!shellViewModel.Program.sc.ReceivedGameStartMessage)
            {
                Thread.Sleep(10);
            }
            shellViewModel.Program.sc.ReceivedGameStartMessage = false;
            while (lobby.IsInGame)
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
