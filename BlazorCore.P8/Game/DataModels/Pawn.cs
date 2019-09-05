using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static BlazorCore.P8.Game.Constants;

namespace BlazorCore.P8.Game.DataModels
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
        public Cell PreviousCell { get; set; } = new Cell();

        public string GetStatus()
        {
            switch(Status)
            {
                case PawnStatus.Alive:
                    return "Alive";
                case PawnStatus.Dead:
                    return "Dead";
                case PawnStatus.Escaped:
                    return "Escaped";
                case PawnStatus.Ready:
                    return "Ready";
                default:
                    return "";
            }
        }
    }
}
