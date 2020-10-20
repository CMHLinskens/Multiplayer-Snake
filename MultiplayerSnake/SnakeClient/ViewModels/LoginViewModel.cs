using GalaSoft.MvvmLight.Command;
using SnakeClient.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SnakeClient.ViewModels
{
    class LoginViewModel : CustomObservableObject
    {
        private ShellViewModel shellViewModel;

        public string Username { get; set; }

        //PasswordBox does not support databinding,
        //which is why a TextBox is currently used
        public string Password { get; set; } = "123";
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
            this.shellViewModel.Initialize();
        }
    }
}
