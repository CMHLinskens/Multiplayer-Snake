using SnakeClient.Utils;
using System.Windows.Media;

namespace SnakeClient.Models
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
                2 => Brushes.DarkGreen,
                3 => Brushes.DarkBlue,
                4 => Brushes.DarkGoldenrod,
                5 => Brushes.DarkOrange,
                6 => Brushes.Green,
                7 => Brushes.Blue,
                8 => Brushes.Yellow,
                9 => Brushes.Orange,
                _ => Brushes.White,
            };
        }
    }
}
