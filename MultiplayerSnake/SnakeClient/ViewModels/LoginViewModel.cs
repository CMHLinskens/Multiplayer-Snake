using GalaSoft.MvvmLight.Command;
using SnakeClient.Utils;
using System;
using System.Collections.Generic;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SnakeClient.ViewModels
{
    class LoginViewModel : CustomObservableObject
    {
        private ShellViewModel shellViewModel;

        public string Username { get; set; }

        public string Password { private get; set; }
        public string LoginMessage { get; set; }
        public ICommand LoginCommand { get; set; }
        public LoginViewModel(ShellViewModel shellViewModel)
        {
            this.shellViewModel = shellViewModel;
            LoginCommand = new RelayCommand(async () => await LoginAsync());
        }

        private async Task LoginAsync()
        {
            if (await Task.Run(() => shellViewModel.Program.Login(Username, Password)))
                LoginSucceeded();
            else
                LoginFailed();
        }

        private void LoginFailed()
        {
            LoginMessage = "Please try again";
        }

        private void LoginSucceeded()
        {
            LoginMessage = "";
            this.shellViewModel.Name = Username;
            this.shellViewModel.Initialize();
        }

    }
}
