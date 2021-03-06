﻿using GalaSoft.MvvmLight.Command;
using SnakeClient.Models;
using SnakeClient.Utils;
using System;
using System.Collections.ObjectModel;
using System.Windows.Ink;

namespace SnakeClient.ViewModels
{
    class SnakeViewModel : CustomObservableObject
    {
        private int[,] gameField;
        public double Width { get; set; } = 1140;
        public double Height { get; set; } = 920;
        private ShellViewModel shellViewModel;
        public StrokeCollection StrokeCollection { get; set; }
        public ObservableCollection<CustomRectangle> Rectangles{ get; set; }

        public SnakeViewModel(ShellViewModel shellViewModel)
        {
            StrokeCollection = new StrokeCollection();
            this.shellViewModel = shellViewModel;
            Rectangles = new ObservableCollection<CustomRectangle>();
        }

        /*
         * Creates the rectangles and adds them to the Rectangles collection. "Draws" the 2d array.
         */
        public void Draw()
        {
            gameField = shellViewModel.Program.sc.GameField;
            double squareRootFieldLength  = Math.Sqrt(gameField.Length);
            App.Current.Dispatcher.Invoke(delegate { Rectangles.Clear(); });
            for (int y = 0; y < squareRootFieldLength; y++)
            {
                for (int x = 0; x < squareRootFieldLength; x++)
                {
                    if (gameField[y, x] > 0)
                    {
                        // Add a rectangle to the collection
                        App.Current.Dispatcher.Invoke(delegate { Rectangles.Add(new CustomRectangle((int)((x / squareRootFieldLength) * Width), (int)((y / squareRootFieldLength) * Height), (int)(Width / squareRootFieldLength), (int)(Height / squareRootFieldLength), Width, Height, gameField[y, x])); });
                    }
                }
            }
        }
    }
}
