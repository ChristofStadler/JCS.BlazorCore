using BlazorCore.Game.DataModels;
using BlazorCore.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorCore.Game;

namespace BlazorCore.Pages
{
    /// <summary>
    /// Process Player actions and input
    /// </summary>
    public class GameZoneComponent : ComponentBase
    {

        [Inject]
        IJSRuntime JSRuntime { get; set; }

        [Inject]
        public LobbyService LobbyService { get; set; }

        [Parameter]
        public Game.DataModels.Player player { get; set; }

        [Parameter]
        public Game.DataModels.Session session { get; set; }

        [JSInvokable]
        public static async Task<string> KeyPressed(string key, string uid, string sessionKey)
        {
            string pressed = "";
            key = key.ToLower();
            if (key.Contains("w"))
            {
                pressed = "up";
                PlayerMove(Constants.Direction.Up, uid, sessionKey);
            }

            else if (key.Contains("s"))
            {
                pressed = "down";
                PlayerMove(Constants.Direction.Down, uid, sessionKey);
            }

            else if (key.Contains("a"))
            {
                pressed = "left";
                PlayerMove(Constants.Direction.Left, uid, sessionKey);
            }

            else if (key.Contains("d"))
            {
                pressed = "right";
                PlayerMove(Constants.Direction.Right, uid, sessionKey);
            }
            else if (key.Contains("z"))
            {
                pressed = "turn-left";
                PlayerTurn(Constants.Direction.Left, uid, sessionKey);
            }
            else if (key.Contains("x"))
            {
                pressed = "turn-right";
                PlayerTurn(Constants.Direction.Right, uid, sessionKey);
            }
            return pressed;
        }

        public static async void PlayerMove(Constants.Direction dir, string uid, string sessionKey)
        {
            LobbyService.Sessions[sessionKey].Players.FirstOrDefault(n => n.UID == uid).Direction = dir;
        }

        public static async void PlayerTurn(Constants.Direction dir, string uid, string sessionKey)
        {

        }

        public void Boost() { }

        public int SVGCoord(int p)
        {
            return p * Constants.CellPixels;
        }
    }
}
