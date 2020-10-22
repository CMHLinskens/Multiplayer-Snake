using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    class Player
    {
        public string Name { get; set; }
        public int Length { get; set; }
        public List<(int y, int x)> Position { get; set; }
        public bool Alive { get; set; }

        public Player(string name)
        {
            Name = name;
            Length = 0;
            Alive = true;
        }
    }
}
