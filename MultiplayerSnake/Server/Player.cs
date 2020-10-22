using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    class Player
    {
        public string Name { get; set; }
        public int Length { get; set; }
        [JsonIgnore]
        public List<(int y, int x)> Position { get; set; }
        [JsonIgnore]
        public bool Alive { get; set; }

        public Player(string name)
        {
            Name = name;
            Length = 3;
            Alive = true;
        }
    }
}
