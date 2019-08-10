using BlazorCore.Game.DataModels;
using BlazorCore.Game.Managers;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static BlazorCore.Game.Constants;

namespace BlazorCore.Services
{
    public class LobbyService
    {
        public static Dictionary<string, Session> Sessions { get; set; } = new Dictionary<string, Session>();
        public static List<Player> Players { get; set; } = new List<Player>();

        #region SessionManagement
        private LevelManager levelManager = new LevelManager();
        public string Create(GameMode mode, Player player, IJSRuntime jsRuntime)
        {
            var uid = Guid.NewGuid().ToString();
            var session = new Session();
            session.UID = uid;
            session.Multiplayer = true;
            session.Mode = mode;
            session.Status = GameStatus.WaitingForPlayers;

            switch (mode)
            {
                case GameMode.TwoPlayer:
                    session.PlayersMax = 2;
                    session.Level = levelManager.CreateLevel(80, 80, 2);
                    session.Players = new Pawn[2];
                    break;
                case GameMode.FourPlayer:
                    session.PlayersMax = 4;
                    session.Level = levelManager.CreateLevel(100, 100, 4);
                    session.Players = new Pawn[4];
                    break;
            }

            Sessions.Add(uid, session);
            Join(uid, player, jsRuntime);

            Task.Run(() => new GameManager(uid).Play());

            return uid;
        }
        public bool Join(string uid, Player player, IJSRuntime jsRuntime)
        {
            if (Sessions[uid]?.Players?.Any(n => n != null && n.UID == player.UID) ?? false) // Player already in game!
            {
                Task.Run(() => jsRuntime.InvokeAsync<object>("App.LocalStorageSet", "Session", uid));
                return true;
            }
            else
            {
                for (int i = 0; i < Sessions[uid].Players.GetLength(0); i++)
                {
                    if (Sessions[uid].Players[i] == null)
                    {
                        Sessions[uid].Players[i] = new Pawn()
                        {
                            UID = player.UID,
                            Name = player.Name,
                            Color = PlayerColors[i],
                            Coord = Sessions[uid].Level.Spawns[i].Coord,
                            Direction = Sessions[uid].Level.Spawns[i].Direction
                        };

                        Task.Run(() => jsRuntime.InvokeAsync<object>("App.LocalStorageSet", "Session", uid));
                        return true;
                    }
                }
            }

            return false; // Game must be full
        }
        #endregion

        #region UserSession
        public async Task<Player> GetCurrentPlayer(IJSRuntime jsRuntime)
        {
            string uid = await GetCurrentPlayerUID(jsRuntime);

            var player = Players.FirstOrDefault(n => n.UID == uid);

            if (player == null)
            {
                player = new Player()
                {
                    UID = uid,
                };
                Players.Add(player);
            }

            return player;
        }

        public async Task<string> GetCurrentPlayerUID(IJSRuntime jsRuntime)
        {
            string uid = await jsRuntime.InvokeAsync<string>("App.LocalStorageGet", "UID");

            if (String.IsNullOrEmpty(uid))
            {
                uid = Guid.NewGuid().ToString();
                await jsRuntime.InvokeAsync<object>("App.LocalStorageSet", "UID", uid);
            }

            return uid;
        }
        #endregion


    }
}
