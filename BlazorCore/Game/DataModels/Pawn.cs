using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static BlazorCore.Game.Constants;

namespace BlazorCore.Game.DataModels
{
    public class Pawn
    {
        public string UID { get; set; }
        public string Color { get; set; }
        public string Name { get; set; }
        public Coord Coord { get; set; }
        public List<Coord> Moves { get; set; }
        public int Boost { get; set; } = 3;
        public int BoostCounter { get; set; } = 0;
        public PawnStatus Status { get; set; }
        public Direction Direction { get; set; }
        public Stats Round { get; set; } = new Stats();
        public Stats Stats { get; set; } = new Stats();
    }
}
