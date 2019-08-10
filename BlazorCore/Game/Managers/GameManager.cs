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

        public int TickRate = 1000;

        private int TickRateIdle = 1000;
        private int TickRatePlay = 100;

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
                    System.Threading.Thread.Sleep(TickRate); // TODO: Tick counter 100, PawnMovement 200, SpeedBoost = 100
                }
            }
            catch(Exception ex)
            {

            }
        }

        public void StartGame()
        {
            // THE GAME IS IN PLAY, SO NOTHING ELSE TO DO
            if (LobbyService.Sessions[UID].Status == Constants.GameStatus.Play)
            {

            }
            // GAME FULL, START TIMER
            else if (LobbyService.Sessions[UID].Status == Constants.GameStatus.WaitingForPlayers && !LobbyService.Sessions[UID].Players.Any(n => n == null))
            {
                LobbyService.Sessions[UID].Status = Constants.GameStatus.Start;
            }
            // COUNT DOWN
            else if (LobbyService.Sessions[UID].Status == Constants.GameStatus.Start && LobbyService.Sessions[UID].StartCounter > 0)
            {
                LobbyService.Sessions[UID].StartCounter--;
            }
            // PLAY GAME!
            else if (LobbyService.Sessions[UID].Status == Constants.GameStatus.Start && LobbyService.Sessions[UID].StartCounter == 0)
            {
                TickRate = TickRatePlay;
                LobbyService.Sessions[UID].Status = Constants.GameStatus.Play;
            }

            // TODO: End game restart

            //if (LobbyService.Sessions[UID].Status != Constants.GameStatus.Ended && !LobbyService.Sessions[UID].Players.Any(n => n == null))
            //{
            //    LobbyService.Sessions[UID].Status = Constants.GameStatus.Play;
            //}
        }

        public void EndGame()
        {
            if (LobbyService.Sessions[UID].Status != Constants.GameStatus.Ended && // Don't end the game if it's already ended
                LobbyService.Sessions[UID].Status != Constants.GameStatus.WaitingForPlayers && // Make sure the game has started
                LobbyService.Sessions[UID].Players.Count(n => n.Status == Constants.PawnStatus.Alive) == 1) // One player left standing
            {
                LobbyService.Sessions[UID].Status = Constants.GameStatus.Ended;
                TickRate = TickRateIdle;

                var winner = LobbyService.Sessions[UID].Players.OrderByDescending(n => n.Round.Score).FirstOrDefault(n => n.Status == Constants.PawnStatus.Alive);
                winner.Stats.Wins++;
                winner.Round.Wins++;

                foreach (var player in LobbyService.Sessions[UID].Players)
                {
                    player.Stats.Games++;

                    if (player.Status == Constants.PawnStatus.Alive)
                        Score(Constants.Score.Survive, player.UID);

                    // Award scores for staying till the end.
                    LobbyService.Players.FirstOrDefault(n => n.UID == player.UID).AddStats(player.Stats);
                }
            }
        }

        public void Score(Constants.Score score, string uid)
        {
            LobbyService.Sessions[UID].Players.FirstOrDefault(n => n.UID == uid).Stats.Score += (int)score;
            LobbyService.Sessions[UID].Players.FirstOrDefault(n => n.UID == uid).Round.Score += (int)score;
            switch(score)
            {
                case Constants.Score.Kill:
                    LobbyService.Sessions[UID].Players.FirstOrDefault(n => n.UID == uid).Stats.Kills++;
                    LobbyService.Sessions[UID].Players.FirstOrDefault(n => n.UID == uid).Round.Kills++;
                    break;
                case Constants.Score.Suicide:
                    LobbyService.Sessions[UID].Players.FirstOrDefault(n => n.UID == uid).Stats.Suicides++;
                    LobbyService.Sessions[UID].Players.FirstOrDefault(n => n.UID == uid).Round.Suicides++;
                    break;
                case Constants.Score.Escape:
                    LobbyService.Sessions[UID].Players.FirstOrDefault(n => n.UID == uid).Stats.Escapes++;
                    LobbyService.Sessions[UID].Players.FirstOrDefault(n => n.UID == uid).Round.Escapes++;
                    break;
                case Constants.Score.Survive:
                    LobbyService.Sessions[UID].Players.FirstOrDefault(n => n.UID == uid).Stats.Survived++;
                    LobbyService.Sessions[UID].Players.FirstOrDefault(n => n.UID == uid).Round.Survived++;
                    break;
                case Constants.Score.PickUp:
                    LobbyService.Sessions[UID].Players.FirstOrDefault(n => n.UID == uid).Stats.Pickups++;
                    LobbyService.Sessions[UID].Players.FirstOrDefault(n => n.UID == uid).Round.Pickups++;
                    break;
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

                        CheckCollision(cell, player.UID);

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

        public void CheckCollision(Cell cell, string uid) {
            switch(cell.Type)
            {
                case Constants.CellType.Player:
                    KillPawn(uid);
                    if (uid == cell.PlayerUID)
                        Score(Constants.Score.Suicide, uid);
                    else
                        Score(Constants.Score.Kill, cell.PlayerUID);
                    break;
                case Constants.CellType.Wall:
                    KillPawn(uid);
                    break;
                case Constants.CellType.Escape:
                    EscapePawn(uid);
                    break;
            }
        }

        public void KillPawn(string uid)
        {
            LobbyService.Sessions[UID].Players.FirstOrDefault(n => n.UID == uid).Status = Constants.PawnStatus.Dead;
            // TODO: Fade out player. Fade out trail.
        }

        public void EscapePawn(string uid)
        {
            // TODO: Fade out trail.
            LobbyService.Sessions[UID].Players.FirstOrDefault(n => n.UID == uid).Status = Constants.PawnStatus.Escaped;
            Score(Constants.Score.Escape, uid);
        }

        public GameManager(string uid)
        {
            UID = uid;
        }
    }
}
