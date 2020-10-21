using System;
using System.Collections.Generic;
using System.Text;

namespace MultiplayerSnake
{
    class Player
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
