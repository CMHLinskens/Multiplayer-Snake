using SnakeClient.Utils;

namespace SnakeClient.ViewModels
{
    class TabControlViewModel : CustomObservableObject
    {
        public LobbyTabViewModel LobbyTabViewModel { get; set; }
        public TabControlViewModel(ShellViewModel shellViewModel)
        {
            LobbyTabViewModel = new LobbyTabViewModel(shellViewModel);
        }
    }
}
