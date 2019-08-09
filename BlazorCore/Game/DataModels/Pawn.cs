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
        public Coord Coord { get; set; }
        public List<Coord> Moves { get; set; }
        public PawnStatus Status { get; set; }
        public Direction Direction { get; set; }
    }
}
