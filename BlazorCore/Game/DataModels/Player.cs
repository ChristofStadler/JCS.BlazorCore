using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorCore.Game.DataModels
{
    public class Player
    {
        public string UID { get; set; }
        public string Name { get; set; } = "Player";
        public int Score { get; set; } = 0;
    }
}
