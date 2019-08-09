using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorCore.Services
{
    public class GameService : AccountService
    {
        public static List<GameSession> Sessions { get; set; } = new List<GameSession>();
        public static List<Player> Players { get; set; } = new List<Player>();

        public static string NewSession(Player player)
        {
            var session = new GameSession(GameMode.TwoPlayer, player);
            Sessions.Add(session);

            return session.UID;
        }

        public async Task<Player> GetPlayer(IJSRuntime jsRuntime)
        {
            string uid = await GetUID(jsRuntime);

            var player = Players.FirstOrDefault(n => n.UID == uid);

            if (player == null)
            {
                player = new Player()
                {
                    UID = uid
                };
                Players.Add(player);
            }

            return player;
        }

        public class GameSession {
            public string UID { get; set; }
            public GameStatus Status { get; set; } = GameStatus.WaitingForPlayers;
            public Pawn[] Players { get; set; }
            public int PlayersMax { get; set; } = 0;
            public string[] Colors { get; set; } = new string[4] { "red","blue","green","orange" };
            public Level Level { get; set; }
            public GameMode Mode { get; set; }
            public bool Multiplayer { get; set; } = true;
            public string GetStatus
            {
                get
                {
                    return GameStatusString(Status);
                }
            }

            public GameSession(GameMode mode, Player player)
            {
                UID = Guid.NewGuid().ToString();
                SetMode(mode);
                Level = new Level(100, 100, PlayersMax);
                Join(player);
            }

            private void SetMode(GameMode mode)
            {
                switch(mode)
                {
                    case GameMode.TwoPlayer:
                        Mode = mode;
                        PlayersMax = 2;
                        Players = new Pawn[PlayersMax];
                        break;
                    case GameMode.FourPlayer:
                        Mode = mode;
                        PlayersMax = 4;
                        Players = new Pawn[PlayersMax];
                        break;
                }
            }

            public bool Join(Player player)
            {
                for (int i = 0; i < Players.GetLength(0); i++)
                {
                    if (Players[i] == null)
                    {
                        Players[i] = new Pawn()
                        {
                            UID = player.UID,
                            Color = Colors[i],
                            Coord = Level.Spawns[i].Coord,
                            Direction = Level.Spawns[i].Direction
                        };
                        return true;
                    }
                }

                return false; // Game must be full
            }
        }



        public static string GameStatusString(GameStatus status)
        {
            switch(status)
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
                default:
                    return "";
            }
        }

        public enum GameStatus { WaitingForPlayers, Play, Paused, Ended, Start }
        public enum GameMode { SinglePlayer, TwoPlayer, FourPlayer, FreeForAll }
        public enum Direction { Up, Down, Left, Right }

        public class Pawn
        {
            public string UID { get; set; }
            public string Color { get; set; }
            public Coord Coord { get; set; }
            public List<Coord> Moves { get; set; }
            public Direction Direction { get; set; }
        }



        public class Coord
        {
            public int X { get; set; }
            public int Y { get; set; }

            public Coord(int x = 0, int y = 0)
            {
                X = x;
                Y = y;
            }
        }

        public class Spawn
        {
            public Coord Coord { get; set; }
            public Direction Direction { get; set; }

            public Spawn(Direction dir, Coord coord)
            {
                Coord = coord;
                Direction = dir;
            }
        }

        public class Cell
        {
            //public Coord Coord { get; set; } 
            //public string ID { get; set; }
            public string PlayerUID { get; set; }
            public string Color { get; set; } = "#eeeeee";
            public CellType Type { get; set; } = CellType.Empty;
        }

        public class Level
        {
            public LevelSize Size { get; set; } 

            public Cell[,] Cells { get; set; }

            public Spawn[] Spawns { get; set; }

            public Level(int width = 100, int height = 100, int spawns = 0)
            {
                Size = new LevelSize(width, height);
                LoadCells(width, height);
                SetSpawns(spawns);
            }

            private void SetSpawns(int spawns)
            {
                Spawns = new Spawn[spawns];

                switch(spawns)
                {
                    case 2:
                        Spawns[0] = new Spawn(Direction.Right, GetRandomCoords((int)(Size.Width * 0.5), Size.Height, 0, 0)); // Left
                        Spawns[1] = new Spawn(Direction.Left, GetRandomCoords(Size.Width, Size.Height, (int)(Size.Width * 0.5), 0)); // Right
                        break;
                    case 4:
                        Spawns[0] = new Spawn(Direction.Right, GetRandomCoords((int)(Size.Width * 0.5), (int)(Size.Height * 0.5), 0, 0)); // Top Left
                        Spawns[1] = new Spawn(Direction.Left, GetRandomCoords(Size.Width, (int)(Size.Height * 0.5), (int)(Size.Width * 0.5), 0)); // Top Right
                        Spawns[2] = new Spawn(Direction.Left, GetRandomCoords(Size.Width, Size.Height, (int)(Size.Width * 0.5), (int)(Size.Height * 0.5))); // Bottom Right
                        Spawns[3] = new Spawn(Direction.Right, GetRandomCoords((int)(Size.Width * 0.5), Size.Height, 0, (int)(Size.Height * 0.5))); // Bottom Left
                        break;
                }
            }

            private Coord GetRandomCoords(int xMax, int yMax, int xMin, int yMin)
            {
                Random r = new Random();
                int x = r.Next(xMin, xMax);
                int y = r.Next(yMin, yMax);

                return new Coord(x,y);
            }

            private void LoadCells(int width, int height)
            {
                Cells = new Cell[Size.Width, Size.Height];
                for (int x = 0; x < Cells.GetLength(0); x++)
                {
                    for (int y = 0; y < Cells.GetLength(1); y++)
                    {
                        Cells[x, y] = new Cell();
                    }
                }
            }
        }

        public class LevelSize
        {
            public int Width { get; set; }

            public int WidthPX { get { return Width * CellPX; } }
            public int Height { get; set; }

            public int HeightPX { get { return Height * CellPX; } }

            public int CellPX = 8; // Cell pixel size

            public LevelSize(int width, int height)
            {
                Width = width;
                Height = height;
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
