using SnakeClient.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SnakeClient.Views
{
    /// <summary>
    /// Interaction logic for GameWindow.xaml
    /// Implements ICloseable so it does not violate the MVVM pattern
    /// if we want to close.
    /// </summary>
    public partial class GameWindow : Window, ICloseable
    {
        public GameWindow()
        {
            InitializeComponent();
        }
    }
}
