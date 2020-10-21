using SnakeClient.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace SnakeClient.Models
{
    class Player : CustomObservableObject
    {
        public string Name { get; set; }
        public int Length { get; set; }

        public Player(string name)
        {
            Name = name;
            Length = 0;
        }
    }
}
