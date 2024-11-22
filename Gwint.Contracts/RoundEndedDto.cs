using Gwint.Api.DTOs;

namespace Gwint.Contracts;

public class RoundEndedDto
{
    public RoundEndedDto(PlayerGameStateDto gameState, string roundWinner)
    {
        GameState = gameState;
        RoundWinner = roundWinner;
    }

    public PlayerGameStateDto GameState { get; set; }
    public string RoundWinner { get; set; }
}