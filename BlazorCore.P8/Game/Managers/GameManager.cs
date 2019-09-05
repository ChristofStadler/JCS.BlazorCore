using BlazorCore.P8.Game.DataModels;
using BlazorCore.P8.Services;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static BlazorCore.P8.Game.Constants;

namespace BlazorCore.P8.Game.Managers
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
            if(LobbyService.Sessions[UID].Status == Constants.GameStatus.WaitingForPlayers)
            {
                for (int i = 0; i < LobbyService.Sessions[UID].Players.GetLength(0); i++)
                {
                    if (LobbyService.Sessions[UID].Players[i]?.Status == Constants.PawnStatus.Ready)
                    {
                        var player = LobbyService.Sessions[UID].Players[i];
                        LobbyService.Sessions[UID].Level.Cells[player.Coord.X, player.Coord.Y] = new Cell(Constants.CellType.PlayerHead, player.UID, player.Color);

                        LobbyService.Sessions[UID].Players[i].Status = Constants.PawnStatus.Alive;
                    }
                }
            }
        }

        public async Task Play()
        {
            try
            {
                while (!LobbyService.Sessions[UID].Players.All(n => n == null))
                {
                    SpawnPlayers(); // Spawns new players.
                    StartGame(); // Start game if conditions met.
                    MovePawns(); // Move pawns if conditions met.
                    EndGame(); // End game if one player left and the rest dead.
                    System.Threading.Thread.Sleep(TickRate); // TODO: Tick counter 100, PawnMovement 200, SpeedBoost = 100
                }

                LobbyService.Sessions[UID].Status = GameStatus.SessionEnded;
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
            // START NEW ROUND
            else if (LobbyService.Sessions[UID].Status == Constants.GameStatus.Ended && !LobbyService.Sessions[UID].Players.Any(n => n.Status != Constants.PawnStatus.Ready))
            {
                NewRound();
            }
        }

        public void NewRound()
        {
            switch (LobbyService.Sessions[UID].Mode)
            {
                case GameMode.TwoPlayer:
                    LobbyService.Sessions[UID].Level = new LevelManager().CreateLevel(80, 80, 2);
                    break;
                case GameMode.FourPlayer:
                    LobbyService.Sessions[UID].Level = new LevelManager().CreateLevel(100, 100, 4);
                    break;
            }

            for (int i = 0; i < LobbyService.Sessions[UID].Players.GetLength(0); i++)
            {
                if(LobbyService.Sessions[UID].Players[i] != null)
                {
                    LobbyService.Sessions[UID].Players[i].Round = new Stats();
                    LobbyService.Sessions[UID].Players[i].Coord = LobbyService.Sessions[UID].Level.Spawns[i].Coord;
                    LobbyService.Sessions[UID].Players[i].Direction = LobbyService.Sessions[UID].Level.Spawns[i].Direction;
                }
            }

            LobbyService.Sessions[UID].StartCounter = 5;
            LobbyService.Sessions[UID].Status = Constants.GameStatus.WaitingForPlayers;
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

                        var coord = new Coord(LobbyService.Sessions[UID].Players[i].Coord); // Record coord before move

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

                        var collission = CheckCollision(cell, player.UID);

                        if(!collission)
                        {
                            LobbyService.Sessions[UID].Level.Cells[player.Coord.X, player.Coord.Y] = new Cell(Constants.CellType.PlayerHead, player.UID, player.Color);

                            // Set Previous
                            LobbyService.Sessions[UID].Level.Cells[coord.X, coord.Y] = new Cell(Constants.CellType.Player, player.UID, player.Color);
                        }
                    }
                }

                MoveCounter++; // Move players every second tick.
                if (MoveCounter == 3) MoveCounter = 1; // Reset to 1 after 2nd tick
            }
        }

        public bool CheckCollision(Cell cell, string uid) {
            switch(cell.Type)
            {
                case Constants.CellType.Player:
                    KillPawn(uid);
                    if (uid == cell.PlayerUID)
                        Score(Constants.Score.Suicide, uid);
                    else
                        Score(Constants.Score.Kill, cell.PlayerUID);
                    return true;
                case Constants.CellType.Wall:
                    KillPawn(uid);
                    return true;
                case Constants.CellType.Escape:
                    EscapePawn(uid);
                    return false;
                default:
                    return false;
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
