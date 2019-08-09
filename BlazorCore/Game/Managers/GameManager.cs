using BlazorCore.Game.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorCore.Game.Managers
{
    /// <summary>
    /// Manages game state
    /// Player moves
    /// Start and End game state
    /// Sets Mode and Satus
    /// </summary>
    public class GameManager
    {
        public Session Session;
        public void MoveLeft() { }
        public void MoveRight() { }
        public void MoveUp() { }
        public void MoveDown() { }

        public void SpawnPlayer() { }

        public async Task Play()
        {
            while (Session.Status != Constants.GameStatus.Ended)
            {
                if(Session.Status == Constants.GameStatus.Play)
                {
                    MovePawns();
                }
                System.Threading.Thread.Sleep(1000);
            }
        }

        public void MovePawns()
        {
            foreach(var pawn in Session.Players)
            {
                switch(pawn.Direction)
                {
                    case Constants.Direction.Left:
                        pawn.Coord.X--;
                        break;
                    case Constants.Direction.Right:
                        pawn.Coord.X++;
                        break;
                    case Constants.Direction.Up:
                        pawn.Coord.Y--;
                        break;
                    case Constants.Direction.Down:
                        pawn.Coord.Y++;
                        break;
                }
            }
        }
    }
}
