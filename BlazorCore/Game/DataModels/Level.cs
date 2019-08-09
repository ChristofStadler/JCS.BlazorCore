﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static BlazorCore.Game.Constants;

namespace BlazorCore.Game.DataModels
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
        public string Color { get; set; }
        public CellType Type { get; set; }
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

        public int WidthPX { get { return Width * CellPX; } }
        public int Height { get; set; }

        public int HeightPX { get { return Height * CellPX; } }

        public int CellPX = CellPixels; // Cell pixel size

        public LevelSize(int width, int height)
        {
            Width = width;
            Height = height;
        }
    }
}
