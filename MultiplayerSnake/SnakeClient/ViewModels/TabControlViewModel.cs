using SnakeClient.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace SnakeClient.ViewModels
{
    class TabControlViewModel : CustomObservableObject
    {
        public LobbyTabViewModel LobbyTabViewModel { get; set; }
        public ChatTabViewModel ChatTabViewModel { get; set; }
        public TabControlViewModel(ShellViewModel shellViewModel)
        {
            LobbyTabViewModel = new LobbyTabViewModel(shellViewModel);
            ChatTabViewModel = new ChatTabViewModel(shellViewModel);
        }
    }
}
