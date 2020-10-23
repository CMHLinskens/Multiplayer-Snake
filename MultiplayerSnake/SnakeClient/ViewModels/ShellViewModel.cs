using SnakeClient.Utils;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Windows;

namespace SnakeClient.ViewModels
{
    class ShellViewModel : CustomObservableObject
    {
        public string Name { get; set; }
        public Client Program { get; set; }
        public Visibility Visibility{ get; set; }
        public CustomObservableObject SelectedViewModel{ get; set; }
        
        public ShellViewModel()
        {
            Program = new Client();
            SelectedViewModel = new LoginViewModel(this);
        }

        public void Initialize()
        {
            SelectedViewModel = new TabControlViewModel(this);
        }

        internal void NavigateToRegisterPage()
        {
            SelectedViewModel = new RegisterViewModel(this);
        }
    }
}
