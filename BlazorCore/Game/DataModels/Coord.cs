using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorCore.Game.DataModels
{
    public class Coord
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Coord(int x = 0, int y = 0)
        {
            X = x;
            Y = y;
        }

        public Coord(Coord coord)
        {
            X = coord.X;
            Y = coord.Y;
        }
    }
}
