﻿using System;
using System.Threading;
using Utils;

namespace SnakeClient
{
    class Program
    {
        public ServerConnection sc { get; }

        public Program()
        {
            sc = new ServerConnection();
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

        internal void RefreshLobbyList()
        {
            sc.RefreshLobbyList();

            while (!sc.ReceivedAllLobbies)
                Thread.Sleep(10);

            return;
        }
    }
}
