using BlazorCore.Game.DataModels;
using BlazorCore.Services;
using Microsoft.AspNetCore.Components;
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
    /// </summary>
    public class GameManager
    {
        [Inject]
        LobbyService LobbyService { get; set; }

        public string UID { get; set; }

        public int MoveCounter = 1;

        public void SpawnPlayers() {
            for (int i = 0; i < LobbyService.Sessions[UID].Players.GetLength(0); i++)
            {
                if (LobbyService.Sessions[UID].Players[i]?.Status == Constants.PawnStatus.Spawn)
                {
                    var player = LobbyService.Sessions[UID].Players[i];
                    LobbyService.Sessions[UID].Level.Cells[player.Coord.X, player.Coord.Y] = new Cell {
                        Color = player.Color,
                        PlayerUID = player.UID,
                        Type = Constants.CellType.Player
                    };

                    LobbyService.Sessions[UID].Players[i].Status = Constants.PawnStatus.Alive;
                }
            }
        }

        public async Task Play()
        {
            try
            {
                while (LobbyService.Sessions[UID].Status != Constants.GameStatus.Ended)
                {
                    SpawnPlayers(); // Spawns new players.
                    StartGame(); // Start game if conditions met.
                    MovePawns(); // Move pawns if conditions met.
                    EndGame(); // End game if one player left and the rest dead.
                    System.Threading.Thread.Sleep(500); // TODO: Tick counter 250, PawnMovement 500, SpeedBoost = 250
                }
            }
            catch(Exception ex)
            {

            }
        }

        public void StartGame()
        {
            if(LobbyService.Sessions[UID].Status != Constants.GameStatus.Ended && !LobbyService.Sessions[UID].Players.Any(n => n == null))
            {
                LobbyService.Sessions[UID].Status = Constants.GameStatus.Play;
            }
        }

        public void EndGame()
        {
            if (LobbyService.Sessions[UID].Status != Constants.GameStatus.Ended && // Don't end the game if it's already ended
                LobbyService.Sessions[UID].Status != Constants.GameStatus.WaitingForPlayers && // Make sure the game has started
                LobbyService.Sessions[UID].Players.Count(n => n.Status == Constants.PawnStatus.Alive) == 1) // One player left standing
            {
                LobbyService.Sessions[UID].Status = Constants.GameStatus.Ended;
            }
        }

        public void MovePawns()
        {
            if (LobbyService.Sessions[UID].Status == Constants.GameStatus.Play)
            {
                for (int i = 0; i < LobbyService.Sessions[UID].Players.GetLength(0); i++)
                {
                    if (LobbyService.Sessions[UID].Players[i].Status == Constants.PawnStatus.Alive && (MoveCounter == 2 || LobbyService.Sessions[UID].Players[i].BoostCounter > 0))
                    {
                        if (LobbyService.Sessions[UID].Players[i].BoostCounter > 0) LobbyService.Sessions[UID].Players[i].BoostCounter--;

                        switch (LobbyService.Sessions[UID].Players[i].Direction)
                        {
                            case Constants.Direction.Left:
                                LobbyService.Sessions[UID].Players[i].Coord.X--;
                                break;
                            case Constants.Direction.Right:
                                LobbyService.Sessions[UID].Players[i].Coord.X++;
                                break;
                            case Constants.Direction.Up:
                                LobbyService.Sessions[UID].Players[i].Coord.Y--;
                                break;
                            case Constants.Direction.Down:
                                LobbyService.Sessions[UID].Players[i].Coord.Y++;
                                break;
                        }

                        var player = LobbyService.Sessions[UID].Players[i];
                        var cell = LobbyService.Sessions[UID].Level.Cells[player.Coord.X, player.Coord.Y];

                        CheckCollision(cell.Type, player.UID);

                        LobbyService.Sessions[UID].Level.Cells[player.Coord.X, player.Coord.Y] = new Cell
                        {
                            Color = player.Color,
                            PlayerUID = player.UID,
                            Type = Constants.CellType.Player
                        };
                    }
                }

                MoveCounter++; // Move players every second tick.
                if (MoveCounter == 3) MoveCounter = 1; // Reset to 1 after 2nd tick
            }
        }

        public void CheckCollision(Constants.CellType cell, string uid) {
            switch(cell)
            {
                case Constants.CellType.Player:
                    KillPawn(uid);
                    break;
                case Constants.CellType.Wall:
                    KillPawn(uid);
                    break;
            }
        }

        public void KillPawn(string uid)
        {
            LobbyService.Sessions[UID].Players.FirstOrDefault(n => n.UID == uid).Status = Constants.PawnStatus.Dead;
        }

        public GameManager(string uid)
        {
            UID = uid;
        }
    }
}
