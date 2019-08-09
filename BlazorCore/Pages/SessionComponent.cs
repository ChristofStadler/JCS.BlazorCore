using BlazorCore.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorCore.Pages
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
        public Game.DataModels.Session session;

        bool loaded = false;

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

        public void NewGame()
        {
            var session = LobbyService.SessionManager.Create(Game.Constants.GameMode.FourPlayer, player);
            LobbyService.Sessions.Add(session);
            session = LobbyService.Sessions.FirstOrDefault(n => n.UID == session.UID);
        }
    }
}
