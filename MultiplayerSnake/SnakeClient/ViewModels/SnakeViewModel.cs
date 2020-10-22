using SnakeClient.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Input;
using Utils;

namespace SnakeClient.ViewModels
{
    class SnakeViewModel : CustomObservableObject
    {
        private int[,] gameField;
        private Direction currentDirection;
        public ICommand KeyDownCommand{ get; set; }
        public ICommand KeyLeftCommand { get; set; }
        public ICommand KeyUpCommand { get; set; }
        public ICommand KeyRightCommand { get; set; }
        public StrokeCollection StrokeCollection { get; set; }

        public SnakeViewModel(ShellViewModel shellViewModel)
        {
            StrokeCollection = new StrokeCollection();
            
        }

        private void Draw()
        {
            //StrokeCollection.Draw()
        }
    }
}
