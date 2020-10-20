using SnakeClient.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace SnakeClient.ViewModels
{
    public class ShellViewModel
    {
        public CustomObservableObject SelectedViewModel{ get; set; }
        public ShellViewModel()
        {
            SelectedViewModel = new LoginViewModel(this);
        }
    }
}
