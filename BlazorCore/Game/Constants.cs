using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorCore.Game
{
    public static class Constants
    {
        public enum GameStatus { WaitingForPlayers, Play, Paused, Ended, Start, SessionEnded }
        public enum GameMode { SinglePlayer, TwoPlayer, FourPlayer, FreeForAll }
        public enum Direction { Up, Down, Left, Right }
        public enum CellType { None, Player, Empty, Fuel, Boost, Energy, Wall, Escape, PlayerHead }
        public enum CellSubType { None, PlayerHead }
        public enum PawnStatus { Ready, Alive, Dead, Escaped, Spawn }
        public enum Score { Escape = 80, Kill = 40, Suicide = 0, Survive = 20, PickUp = 10 }

        public static readonly int CellPixels = 8;

        public static readonly int TickRate = 200;

        public static readonly string[] PlayerColors = { "red", "blue", "green", "orange" };

        public static GameMode GetMode(string mode)
        {
            switch (mode)
            {
                case "2player":
                    return GameMode.TwoPlayer;
                case "4player":
                    return GameMode.FourPlayer;
                default:
                    return GameMode.TwoPlayer;
            }
        }

        public static string ColorClass(string color)
        {
            switch (color)
            {
                case "red":
                    return "bg-danger";
                case "blue":
                    return "bg-primary";
                case "green":
                    return "bg-success";
                case "orange":
                    return "bg-warning";
                default:
                    return "";
            }
        }
    }
}
