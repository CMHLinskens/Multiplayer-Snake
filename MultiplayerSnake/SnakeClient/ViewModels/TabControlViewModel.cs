using SnakeClient.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace SnakeClient.ViewModels
{
    public class TabControlViewModel : CustomObservableObject
    {
        public LobbyTabViewModel LobbyTabViewModel { get; set; }
        public ChatTabViewModel ChatTabViewModel { get; set; }
        public TabControlViewModel()
        {
            LobbyTabViewModel = new LobbyTabViewModel();
            ChatTabViewModel = new ChatTabViewModel();
        }
    }
}
