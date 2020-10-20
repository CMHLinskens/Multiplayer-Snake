using GalaSoft.MvvmLight.Command;
using SnakeClient.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace SnakeClient.ViewModels
{
    public class LoginViewModel : CustomObservableObject
    {
        private ShellViewModel shellViewModel;

        public string Username { get; set; }

        //PasswordBox does not support databinding,
        //which is why a TextBox is currently used
        public string Password { get; set; }
        public string LoginMessage { get; set; }
        public ICommand LoginCommand { get; set; }
        public LoginViewModel(ShellViewModel shellViewModel)
        {
            this.shellViewModel = shellViewModel;
            LoginCommand = new RelayCommand(() => Login());
        }

        private async void Login()
        {
            //ServerConnection Login Request, preferably boolean for async or something

        }

        private void LoginFailed()
        {
            LoginMessage = "Please try again";
        }

        private void LoginSucceeded()
        {
            
        }
    }
}
