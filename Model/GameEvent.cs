using System;

namespace gbac_baseball.web.Model
{

    public class GameEvent
    {
        public string GameId { get; set; }
        public string HomeTeam { get; set; }
        public DateTime Date { get; set; }
        public int GameNumber { get; set; }
        public int GameEventNumber { get; set; }
        public string VisitingTeam { get; set; }
        public int Innin { get; set; }
        public int IsHomeTeamAtBat { get; set; }
        public int Outs { get; set; }
        public int Balls { get; set; }
        public int Strikes { get; set; }
        public string PitchSequence { get; set; }
        public int VisitorScore { get; set; }
        public int HomeScore { get; set; }
        public string Batter { get; set; }
        public string Bats { get; set; }
        public string Pitcher { get; set; }
        public string Throws { get; set; }
        public string EventText { get; set; }
        public string IsLeadoff { get; set; }
        public string IsPinchHit { get; set; }
        public int DefensivePosition { get; set; }
        public int LineupPosition { get; set; }
        public int EventType { get; set; }
        public string IsBatterEvent { get; set; }
        public string IsAtBat { get; set; }
        public int HitValue { get; set; }
        public string IsSacrificeHit { get; set; }
        public string IsSacrificeFly { get; set; }
        public int OutsOnPlay { get; set; }
        public int RbiOnPlay { get; set; }
        public string IsWildPitch { get; set; }
        public string IsPassedBall { get; set; }
        public string BattedBallType { get; set; }
        public string IsBunt { get; set; }
        public string HitLocation { get; set; }
        public int NumberOfErrors { get; set; }
        public string IsNewGame { get; set; }
        public string IsEndOfGame { get; set; }
    }

}