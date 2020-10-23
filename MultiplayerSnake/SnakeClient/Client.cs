using System;
using System.Threading;
using System.Threading.Tasks;
using Utils;

namespace SnakeClient
{
    class Client
    {
        public ServerConnection sc { get; }

        public Client()
        {
            sc = new ServerConnection();
        }

        internal bool Register(string username, string password)
        {
            sc.Register(username, password);

            while (!sc.ReceivedRegisterMessage)
                Thread.Sleep(10);

            return sc.LoggedIn;
        }

        internal bool Login(string username, string password)
        {
            sc.Login(username, password);

            while (!sc.ReceivedLoginMessage)
                Thread.Sleep(10);

            return sc.LoggedIn;
        }

        internal bool CreateLobby(string lobbyName, string gameOwner, int maxPlayers, MapSize mapSize)
        {
            sc.CreateLobby(lobbyName, gameOwner, maxPlayers, mapSize);

            while (!sc.ReceivedLobbyCreateMessage)
                Thread.Sleep(10);

            return sc.ReceivedLobbyCreateMessage;
        }

        internal bool JoinLobby(string lobbyName, string playerName)
        {
            sc.ConnectToLobby(lobbyName, playerName);

            while (!sc.ReceivedLobbyJoinMessage)
                Thread.Sleep(10);

            return sc.ReceivedLobbyJoinMessage;
        }

        internal bool LeaveLobby(string lobbyName, string playerName)
        {
            sc.LeaveLobby(lobbyName, playerName);

            while (!sc.ReceivedLobbyLeaveMessage)
                Thread.Sleep(10);

            return sc.ReceivedLobbyLeaveMessage;
        }

        internal void RefreshLobbyList()
        {
            sc.RefreshLobbyList();

            while (!sc.ReceivedAllLobbies)
                Thread.Sleep(10);

            return;
        }

        internal void RefreshSpecificLobby()
        {
            sc.RefreshSpecificLobby();

            while (!sc.ReceivedLobbyRefresh)
                Thread.Sleep(10);

            return;
        }

        internal void StartGame()
        {
            sc.StartGame();
        }

        internal void StopGame()
        {
            sc.StopGame();
        }

        internal string ChatRefresh()
        {
            while (!sc.ReceivedNewChatMessage)
                Thread.Sleep(100);

            sc.ReceivedNewChatMessage = false;
            return sc.NewChat;
        }
    }
}
