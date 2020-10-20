using SnakeClient.Models;
using SnakeClient.Utils;
using System.Collections.ObjectModel;

namespace SnakeClient.ViewModels
{
    public class LobbyViewModel
    {
        public string Name { get; set; }
        public ObservableCollection<Player> Players { get; set; }
        public bool IsInGame { get; set; }
        public int MaxPlayers { get; set; }
        public string Owner { get; set; }
        public Direction Direction{ get; set; }
        public MapSize MapSize { get; set; }
        public LobbyViewModel()
        {

        }

        private void RequestStartGame()
        {

        }
    }
}