using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace SnakeClient.Utils
{
    class CustomRectangle : CustomObservableObject
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public double CanvasWidth { get; set; }
        public double CanvasHeight { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public Brush Color { get; set; }
        public CustomRectangle(int x, int y, int width, int height, double canvasWidth, double canvasHeight, int value)
        {
            Width = width;
            Height = height;
            CanvasWidth = canvasWidth;
            CanvasHeight = canvasHeight;
            X = x;
            Y = y;

            Color = value switch
            {
                1 => Brushes.Red,
                2 => Brushes.Green,
                3 => Brushes.Blue,
                4 => Brushes.Yellow,
                5 => Brushes.Orange,
                _ => Brushes.White,
            };
        }
    }
}
