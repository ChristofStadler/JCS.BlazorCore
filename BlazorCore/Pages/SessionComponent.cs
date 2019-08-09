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
        public GameService GameService { get; set; }
        
        public GameService.Player player;

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
                player = await GameService.GetPlayer(JSRuntime);
                loaded = true;

                StateHasChanged();
            }
        }
    }
}
