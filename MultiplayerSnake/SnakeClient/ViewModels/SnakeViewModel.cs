using GalaSoft.MvvmLight.Command;
using SnakeClient.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using Utils;

namespace SnakeClient.ViewModels
{
    class SnakeViewModel : CustomObservableObject
    {
        private int[,] gameField;
        public double Width { get; set; } = 1140;
        public double Height { get; set; } = 920;
        private DrawingContext drawingContext;
        private ShellViewModel shellViewModel;
        public StrokeCollection StrokeCollection { get; set; }
        public ObservableCollection<CustomRectangle> Rectangles{ get; set; }

        public SnakeViewModel(ShellViewModel shellViewModel)
        {
            StrokeCollection = new StrokeCollection();
            this.shellViewModel = shellViewModel;
            // Bind the key events


            this.drawingContext = new DrawingVisual().RenderOpen();
            Rectangles = new ObservableCollection<CustomRectangle>();

            //Test
            //gameField = new int[16, 16];
            //for (int y = 0; y < 16; y++)
            //{
            //    for (int x = 0; x < 16; x++)
            //    {
            //        gameField[y, x] = 0;
            //    }
            //}
            //
            //gameField[3, 3] = 2;
            //gameField[3, 4] = 2;
            //gameField[3, 5] = 2;
            //gameField[7, 6] = 3;
            //gameField[8, 6] = 3;
            //gameField[9, 6] = 3;
        }

        /*
         * Sets new move direction.
         */


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
