using SnakeClient.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SnakeClient.Utils
{
    interface ILobbyViewModel
    {
        public string Name { get; set; }
        public Lobby Lobby{ get; set; }
    }
}
