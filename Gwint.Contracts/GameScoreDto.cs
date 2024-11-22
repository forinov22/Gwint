namespace Gwint.Contracts;

public struct GameScoreDto
{
    public int PlayerTotalScore { get; set; }
    public int OpponentTotalScore { get; set; }
    public int PlayerRoundWins { get; set; }
    public int OpponentRoundWins { get; set; }
}