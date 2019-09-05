using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static BlazorCore.P8.Game.Constants;

namespace BlazorCore.P8.Game.DataModels
{
    public class Level
    {
        public LevelSize Size { get; set; }

        public Cell[,] Cells { get; set; }

        public Spawn[] Spawns { get; set; }
    }

    public class Cell
    {
        public string PlayerUID { get; set; }
        public CellType Type { get; set; } = CellType.Empty;
        public DateTime Created { get; set; } = DateTime.Now;
        public string CssClass { get ; set; }

        public Cell(CellType type = CellType.Empty, string uid = "", string color = "")
        {
            var colorClass = String.IsNullOrEmpty(color) ? "" : " gz-" + color;

            PlayerUID = uid;

            switch(type)
            {
                case CellType.Player:
                    Type = type;
                    CssClass = "gz-player" + colorClass;
                    break;
                case CellType.PlayerHead:
                    Type = CellType.Player;
                    CssClass = "gz-player-head" + colorClass;
                    break;
                case CellType.Empty:
                    Type = type;
                    CssClass = "gz-empty";
                    break;
                case CellType.Escape:
                    Type = type;
                    CssClass = "gz-escape";
                    break;
                case CellType.Wall:
                    Type = type;
                    CssClass = "gz-wall";
                    break;
            }
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

    public class LevelSize
    {
        public int Width { get; set; }

        public int WidthPX { get { return Width * CellPX + CellPX; } }
        public int Height { get; set; }

        public int HeightPX { get { return Height * CellPX + CellPX; } }

        public int CellPX = CellPixels; // Cell pixel size

        public LevelSize(int width, int height)
        {
            Width = width;
            Height = height;
        }
    }
}
