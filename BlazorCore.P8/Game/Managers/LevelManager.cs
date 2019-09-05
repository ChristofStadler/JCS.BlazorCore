using BlazorCore.P8.Game.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static BlazorCore.P8.Game.Constants;

namespace BlazorCore.P8.Game.Managers
{
    /// <summary>
    /// Creates levels
    /// </summary>
    public class LevelManager
    {
        public Level CreateLevel(int width, int height, int spawns)
        {
            var level = new Level();
            level.Size = new LevelSize(width, height);
            level.Cells = GenerateCells(width, height);
            level.Spawns = GenerateSpawns(width, height, spawns);
            return level;
        }
        private Spawn[] GenerateSpawns(int width, int height, int spawns)
        {
            var _spawns = new Spawn[spawns];

            switch (spawns)
            {
                case 2:
                    _spawns[0] = new Spawn(Direction.Right, GetRandomCoords((int)(width * 0.5), height - 5, 5, 5)); // Left
                    _spawns[1] = new Spawn(Direction.Left, GetRandomCoords(width-5, height-5, (int)(width * 0.5), 5)); // Right
                    break;
                case 4:
                    _spawns[0] = new Spawn(Direction.Right, GetRandomCoords((int)(width * 0.5), (int)(height * 0.5), 5, 5)); // Top Left
                    _spawns[1] = new Spawn(Direction.Left, GetRandomCoords(width-5, (int)(height * 0.5), (int)(width * 0.5), 5)); // Top Right
                    _spawns[2] = new Spawn(Direction.Left, GetRandomCoords(width-5, height-5, (int)(width * 0.5), (int)(height * 0.5))); // Bottom Right
                    _spawns[3] = new Spawn(Direction.Right, GetRandomCoords((int)(width * 0.5), height-5, 5, (int)(height * 0.5))); // Bottom Left
                    break;
            }

            return _spawns;
        }

        private Cell[,] GenerateCells(int width, int height)
        {
            var _cells = new Cell[width+1, height+1];
            for (int x = 0; x < _cells.GetLength(0); x++)
            {
                for (int y = 0; y < _cells.GetLength(1); y++)
                {
                    // ESCAPE ZONES
                    if (x == 0) // Escape Zone Left
                        _cells[x, y] = new Cell(CellType.Escape);
                    else if (y == 0) // Escape Zone Top
                        _cells[x, y] = new Cell(CellType.Escape);
                    else if (x == width) // Escape Zone Right
                        _cells[x, y] = new Cell(CellType.Escape);
                    else if (y == height) // Escape Zone Bottom
                        _cells[x, y] = new Cell(CellType.Escape);
                    
                    // WALL
                    else if (x == 1) // Wall Left
                        _cells[x, y] = new Cell(CellType.Wall);
                    else if (y == 1) // Wall Top
                        _cells[x, y] = new Cell(CellType.Wall);
                    else if (x == width-1) // Wall Right
                        _cells[x, y] = new Cell(CellType.Wall);
                    else if (y == height-1) // Wall Bottom
                        _cells[x, y] = new Cell(CellType.Wall);

                    // EMPTY
                    else
                        _cells[x, y] = new Cell();
                }
            }

            return _cells;
        }

        #region Helpers
        private Coord GetRandomCoords(int xMax, int yMax, int xMin, int yMin)
        {
            Random r = new Random();
            int x = r.Next(xMin, xMax);
            int y = r.Next(yMin, yMax);

            return new Coord(x, y);
        }
        #endregion
    }
}
