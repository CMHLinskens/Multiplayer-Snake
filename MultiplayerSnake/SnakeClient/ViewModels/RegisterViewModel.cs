using GalaSoft.MvvmLight.Command;
using SnakeClient.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SnakeClient.ViewModels
{
    class RegisterViewModel : CustomObservableObject
    {
        private ShellViewModel shellViewModel;
        private string _password1;
        private string _password2;
        private string _username;
        public string Password1
        {
            get { return _password1; }
            set
            {
                _password1 = value;
                RegisterMessage = "";
            }
        }
        public string Password2
        {
            get { return _password2; }
            set
            {
                _password2 = value;
                RegisterMessage = "";
            }
        }
        public string Username
        {
            get { return _username; }
            set { _username = value;
                RegisterMessage = "";
            }
        }
        public string PasswordMatchMessage { get; set; }
        public string RegisterMessage { get; set; }
        public ICommand RegisterCommand { get; set; }
        public RegisterViewModel(ShellViewModel shellViewModel)
        {
            this.shellViewModel = shellViewModel;
            RegisterCommand = new RelayCommand(RegisterAsync);
        }

        private async void RegisterAsync()
        {
            if (Password1.Equals(Password2))
            {
                if (await Task.Run(() => shellViewModel.Program.Register(Username, Password1)))
                    RegisterSucceeded();
                else
                    RegisterFailed();
            }
            else
            {
                RegisterMessage = "Passwords don't match.";
            }
        }

        private void RegisterFailed()
        {
            RegisterMessage = "Username is not unique.";
        }

        private void RegisterSucceeded()
        {
            this.shellViewModel.Name = Username;
            this.shellViewModel.Initialize();
        }
    }
}
