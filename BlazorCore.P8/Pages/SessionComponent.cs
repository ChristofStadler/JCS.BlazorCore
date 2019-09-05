using BlazorCore.P8.Game.DataModels;
using BlazorCore.P8.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorCore.P8.Pages
{
    public class SessionComponent : ComponentBase
    {
        [Inject]
        IComponentContext ComponentContext { get; set; }

        [Inject]
        IJSRuntime JSRuntime { get; set; }

        [Inject]
        public LobbyService LobbyService { get; set; }

        public Game.DataModels.Player player;
        //public string sessionKey;
        public Game.DataModels.Session session;

        public bool loaded = false;

        protected override async Task OnAfterRenderAsync()
        {
            // TEMPORARY: Currently we need this guard to avoid making the interop
            // call during prerendering. Soon this will be unnecessary because we
            // will change OnAfterRenderAsync so that it won't run during the
            // prerendering phase.
            if (!ComponentContext.IsConnected)
            {
                return;
            }

            if (player == null && loaded == false)
            {
                player = await LobbyService.GetCurrentPlayer(JSRuntime);
                loaded = true;

                StateHasChanged();
            }
        }

        public void NewGame(Game.Constants.GameMode mode)
        {
            var uid = LobbyService.Create(mode, player, JSRuntime);
            session = LobbyService.Sessions[uid];
        }

        public bool JoinGame(string uid)
        {
            if(LobbyService.Join(uid, player, JSRuntime))
            {
                session = LobbyService.Sessions[uid];
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool QuickPlay(Game.Constants.GameMode mode)
        {
            // Find game with spots open.
            var sesh = LobbyService.Sessions.FirstOrDefault(n => n.Value.Mode == mode && n.Value.Status == Game.Constants.GameStatus.WaitingForPlayers && n.Value.Players.Any(x => x == null)).Value;

            if (sesh == null) // Create a new game if none exist.
            {
                NewGame(mode);
                return true;
            }
            else
            {
                if (LobbyService.Join(sesh.UID, player, JSRuntime))
                {
                    session = LobbyService.Sessions[sesh.UID];
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
