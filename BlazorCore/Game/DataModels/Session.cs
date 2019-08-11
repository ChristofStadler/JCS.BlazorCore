using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static BlazorCore.Game.Constants;

namespace BlazorCore.Game.DataModels
{
    public class Session
    {
        public string UID { get; set; }
        public GameStatus Status { get; set; }
        public Pawn[] Players { get; set; }
        public int PlayersMax { get; set; }

        public Level Level { get; set; }
        public GameMode Mode { get; set; }
        public bool Multiplayer { get; set; }

        public int StartCounter { get; set; } = 5;

        public Task Task { get; set; }

        public string GetStatus()
        {
            switch (Status)
            {
                case GameStatus.WaitingForPlayers:
                    return "Waiting for players";
                case GameStatus.Play:
                    return "Play";
                case GameStatus.Start:
                    return "Start";
                case GameStatus.Ended:
                    return "Ended";
                case GameStatus.Paused:
                    return "Paused";
                case GameStatus.SessionEnded:
                    return "Session Ended";
                default:
                    return "";
            }
        }
    }
}
