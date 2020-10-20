using SnakeClient.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace SnakeClient.ViewModels
{
    public class LobbyTabViewModel : CustomObservableObject
    {
        public ObservableCollection<LobbyViewModel> Lobbies{ get; set; }
        public LobbyTabViewModel()
        {

        }
    }
}
