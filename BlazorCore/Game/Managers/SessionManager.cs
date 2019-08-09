using BlazorCore.Game.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static BlazorCore.Game.Constants;

namespace BlazorCore.Game.Managers
{
    /// <summary>
    /// CRUD Sessions
    /// </summary>
    public class SessionManager
    {
        private LevelManager levelManager = new LevelManager();
        public Session Create(GameMode mode, Player player)
        {
            var session = new Session();
            session.UID = Guid.NewGuid().ToString();
            session.Multiplayer = true;
            session.Mode = mode;
            session.Status = GameStatus.WaitingForPlayers;

            switch (mode)
            {
                case GameMode.TwoPlayer:
                    session.PlayersMax = 2;
                    session.Level = levelManager.CreateLevel(50, 50, 2);
                    session.Players = new Pawn[2];
                    break;
                case GameMode.FourPlayer:
                    session.PlayersMax = 4;
                    session.Level = levelManager.CreateLevel(100, 100, 4);
                    session.Players = new Pawn[4];
                    break;
            }
             
            return session;
        }
        public bool Join(Session session, Player player)
        {
            for (int i = 0; i < session.Players.GetLength(0); i++)
            {
                if (session.Players[i] == null)
                {
                    session.Players[i] = new Pawn()
                    {
                        UID = player.UID,
                        Color = PlayerColors[i],
                        Coord = session.Level.Spawns[i].Coord,
                        Direction = session.Level.Spawns[i].Direction
                    };

                    
                    return true;
                }
            }

            return false; // Game must be full
        }
    }
}
