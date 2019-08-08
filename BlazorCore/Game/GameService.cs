using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorCore.Game
{
    public class GameService
    {
        public static List<Session> Sessions { get; set; } = new List<Session>();
        public static List<Player> Players { get; set; } = new List<Player>();

        public static void NewSession(string playerUID)
        {
            var session = new Session();
            session.Players.Add(new Pawn
            {
                UID = playerUID
            });

            Sessions.Add(session);
        }

        public class Session {
            public string UID { get; set; }
            public SessionStatus Status { get; set; } = SessionStatus.WaitingForPlayers;
            public List<Pawn> Players { get; set; } = new List<Pawn>();
            public Level Level { get; set; } = new Level();
            public SessionMode Mode { get; set; }
            public bool Multiplayer { get; set; }
            public string GetStatus
            {
                get
                {
                    return SessionStatusString(Status);
                }
            }

            public Session()
            {
                UID = Guid.NewGuid().ToString();
            }
        }

        public enum SessionStatus { WaitingForPlayers, Play, Paused, Ended, Start }

        public static string SessionStatusString(SessionStatus status)
        {
            switch(status)
            {
                case SessionStatus.WaitingForPlayers:
                    return "Waiting for players";
                case SessionStatus.Play:
                    return "Play";
                case SessionStatus.Start:
                    return "Start";
                case SessionStatus.Ended:
                    return "Ended";
                case SessionStatus.Paused:
                    return "Paused";
                default:
                    return "";
            }
        }

        public enum SessionMode { SinglePlayer, TwoPlayer, FourPlayer, FreeForAll }

        public class Pawn
        {
            public string UID { get; set; }
            public string Color { get; set; }
            public Coord Coord { get; set; }
            public List<Coord> Moves { get; set; }
            public Direction Direction { get; set; }
        }

        public enum Direction { Up, Down, Left, Right }

        public class Coord
        {
            public int X { get; set; }
            public int Y { get; set; }

            public Coord(int x, int y)
            {
                X = x;
                Y = y;
            }
        }
        public class Level
        {
            public int SizeX { get; set; } = 100;
            public int SizeY { get; set; } = 100;
            public object[,] Cells { get; set; }

            public Level()
            {
                Cells = new object[SizeX - 1, SizeY - 1];
            }
        }

        public enum CellType { Player, Empty, Fuel, Boost, Energy }

        public class Energy
        {
            public string Color { get; set; }
        }

        public class Player
        {
            public string UID { get; set; }
            public string Name { get; set; } = "Player";
            public int Score { get; set; } = 0;
        }
    }
}
