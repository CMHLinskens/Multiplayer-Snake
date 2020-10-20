using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SnakeClient.Utils
{
    public class CustomObservableObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
