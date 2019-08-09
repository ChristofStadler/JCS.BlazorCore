using BlazorCore.Game.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static BlazorCore.Game.Constants;

namespace BlazorCore.Game.Managers
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
                    _spawns[0] = new Spawn(Direction.Right, GetRandomCoords((int)(width * 0.5), height, 0, 0)); // Left
                    _spawns[1] = new Spawn(Direction.Left, GetRandomCoords(width, height, (int)(width * 0.5), 0)); // Right
                    break;
                case 4:
                    _spawns[0] = new Spawn(Direction.Right, GetRandomCoords((int)(width * 0.5), (int)(height * 0.5), 0, 0)); // Top Left
                    _spawns[1] = new Spawn(Direction.Left, GetRandomCoords(width, (int)(height * 0.5), (int)(width * 0.5), 0)); // Top Right
                    _spawns[2] = new Spawn(Direction.Left, GetRandomCoords(width, height, (int)(width * 0.5), (int)(height * 0.5))); // Bottom Right
                    _spawns[3] = new Spawn(Direction.Right, GetRandomCoords((int)(width * 0.5), height, 0, (int)(height * 0.5))); // Bottom Left
                    break;
            }

            return _spawns;
        }

        private Cell[,] GenerateCells(int width, int height)
        {
            var _cells = new Cell[width, height];
            for (int x = 0; x < _cells.GetLength(0); x++)
            {
                for (int y = 0; y < _cells.GetLength(1); y++)
                {
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
