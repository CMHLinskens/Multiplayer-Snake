using SnakeClient.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace SnakeClient.ViewModels
{
    class ShellViewModel : CustomObservableObject
    {
        public Program Program { get; set; }
        public CustomObservableObject SelectedViewModel{ get; set; }
        
        public ShellViewModel()
        {
            Program = new Program();
            SelectedViewModel = new LoginViewModel(this);
        }

        public void Initialize()
        {
            SelectedViewModel = new TabControlViewModel(this);
        }
    }
}
