using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorCore.P8.Game.DataModels
{
    public class Player
    {
        public string UID { get; set; }
        public string Name { get; set; } = "Player";
        public Stats Stats { get; set; } = new Stats();

        public void AddStats(Stats stats)
        {
            Stats.Score += stats.Score;
            Stats.Wins += stats.Wins; // You win if your score is the highest and you either survived or escaped.
            Stats.Games += stats.Games;
            Stats.Kills += stats.Kills;
            Stats.Escapes += stats.Score;
        }

        // TODO: Stats for pickups
        //public int Pickups { get; set; } = 0;
    }
    public class Stats
    {
        public int Score { get; set; } = 0;
        public int Wins { get; set; } = 0;
        public int Games { get; set; } = 0;
        public int Kills { get; set; } = 0;
        public int Escapes { get; set; } = 0;
        public int Survived { get; set; } = 0;
        public int Pickups { get; set; } = 0;
        public int Suicides { get; set; } = 0;
    }
}
