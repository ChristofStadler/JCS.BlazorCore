﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorCore.Game
{
    public static class Constants
    {
        public enum GameStatus { WaitingForPlayers, Play, Paused, Ended, Start }
        public enum GameMode { SinglePlayer, TwoPlayer, FourPlayer, FreeForAll }
        public enum Direction { Up, Down, Left, Right }
        public enum CellType { Player, Empty, Fuel, Boost, Energy, Wall, Escape }
        public enum PawnStatus { Spawn, Alive, Dead}

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
    }
}
