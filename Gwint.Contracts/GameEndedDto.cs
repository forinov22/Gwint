namespace Gwint.Contracts;

public class GameEndedDto
{
    public GameEndedDto(string gameWinner)
    {
        GameWinner = gameWinner;
    }

    public string GameWinner { get; set; }
}