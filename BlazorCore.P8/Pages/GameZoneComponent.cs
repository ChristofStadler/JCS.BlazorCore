using BlazorCore.P8.Game.DataModels;
using BlazorCore.P8.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorCore.P8.Game;

namespace BlazorCore.P8.Pages
{
    /// <summary>
    /// Process Player actions and input
    /// </summary>
    public class GameZoneComponent : ComponentBase
    {
        [Inject]
        IComponentContext ComponentContext { get; set; }

        [Inject]
        IJSRuntime JSRuntime { get; set; }

        [Inject]
        public LobbyService LobbyService { get; set; }

        [Parameter]
        public Game.DataModels.Player player { get; set; }

        [Parameter]
        public Game.DataModels.Session session { get; set; }

        public string StatusMessage { get; set; } = "";
        public string StatusMessageSub { get; set; } = "";

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

            InputListener();
        }

        [JSInvokable]
        public static async Task<string> KeyPressed(string key, string uid, string sessionKey)
        {
            string pressed = "";
            key = key.ToLower();

            // AWSD KEYS
            if (key == "w" || key == "up")
            {
                pressed = "up";
                PlayerMove(Constants.Direction.Up, uid, sessionKey);
            }

            else if (key == "s" || key == "down")
            {
                pressed = "down";
                PlayerMove(Constants.Direction.Down, uid, sessionKey);
            }

            else if (key == "a" || key == "left")
            {
                pressed = "left";
                PlayerMove(Constants.Direction.Left, uid, sessionKey);
            }

            else if (key == "d" || key == "right")
            {
                pressed = "right";
                PlayerMove(Constants.Direction.Right, uid, sessionKey);
            }


            // LEFT RIGHT KEYS
            else if (key == "z")
            {
                pressed = "turn-left";
                PlayerTurn(Constants.Direction.Left, uid, sessionKey);
            }
            else if (key == "x")
            {
                pressed = "turn-right";
                PlayerTurn(Constants.Direction.Right, uid, sessionKey);
            }


            // READY / BRAKE KEY
            else if (key == " ")
            {
                pressed = "space";
                PlayerReady(uid, sessionKey);
            }


            // BOOST KEY
            else if (key == "shift")
            {
                pressed = "shift";
                PlayerBoost(uid, sessionKey);
            }
            return pressed;
        }

        public static async void PlayerMove(Constants.Direction dir, string uid, string sessionKey = "")
        {
            var player = LobbyService.Sessions[sessionKey]?.Players?.FirstOrDefault(n => n.UID == uid);
            if(player != null)
            {
                // Don't reverse into yourself.
                if(player.Direction == Constants.Direction.Left && dir == Constants.Direction.Right) return;
                if(player.Direction == Constants.Direction.Right && dir == Constants.Direction.Left) return;
                if(player.Direction == Constants.Direction.Up && dir == Constants.Direction.Down) return;
                if(player.Direction == Constants.Direction.Down && dir == Constants.Direction.Up) return;

                player.Direction = dir;
            }
        }

        public static async void PlayerReady(string uid, string sessionKey = "")
        {
            if (LobbyService.Sessions[sessionKey] != null && LobbyService.Sessions[sessionKey].Status == Constants.GameStatus.Ended)
                LobbyService.Sessions[sessionKey].Players.FirstOrDefault(n => n.UID == uid).Status = Constants.PawnStatus.Ready;
        }

        public static async void PlayerTurn(Constants.Direction dir, string uid, string sessionKey = "")
        {
            var player = LobbyService.Sessions[sessionKey]?.Players?.FirstOrDefault(n => n.UID == uid);
            if (player != null)
            {
                if (dir == Constants.Direction.Left)
                {
                    if (player.Direction == Constants.Direction.Left)
                        player.Direction = Constants.Direction.Down;
                    else if (player.Direction == Constants.Direction.Down)
                        player.Direction = Constants.Direction.Right;
                    else if (player.Direction == Constants.Direction.Right)
                        player.Direction = Constants.Direction.Up;
                    else if (player.Direction == Constants.Direction.Up)
                        player.Direction = Constants.Direction.Left;
                }
                else if (dir == Constants.Direction.Right)
                {
                    if (player.Direction == Constants.Direction.Left)
                        player.Direction = Constants.Direction.Up;
                    else if (player.Direction == Constants.Direction.Up)
                        player.Direction = Constants.Direction.Right;
                    else if (player.Direction == Constants.Direction.Right)
                        player.Direction = Constants.Direction.Down;
                    else if (player.Direction == Constants.Direction.Down)
                        player.Direction = Constants.Direction.Right;
                }
            }
        }

        public static void PlayerBoost(string uid, string sessionKey = "") { }

        public int SVGCoord(int p)
        {
            return p * Constants.CellPixels;
        }

        public void InputListener()
        {
            Task.Run(() => JSRuntime.InvokeAsync<string>("App.InputListener"));
        }
    }
}
