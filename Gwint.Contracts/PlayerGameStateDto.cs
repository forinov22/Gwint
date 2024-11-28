using System.Text.Json.Serialization;

using Gwint.Contracts;
using Gwint.Lib;

namespace Gwint.Api.DTOs
{
    public class PlayerGameStateDto
    {
        public PlayerDto Player { get; set; }
        public OpponentDto Opponent { get; set; }
        public GameScoreDto GameScore { get; set; }
        public GameStatus GameStatus { get; set; }
        public bool IsPlayerTurn { get; set; }
        public Card? LastPlayedCard { get; set; }
        public IEnumerable<WeatherCard> WeatherCards { get; set; } = [];

        public PlayerGameStateDto(
            PlayerDto player,
            OpponentDto opponent,
            GameScoreDto gameScore,
            GameStatus gameStatus,
            bool isPlayerTurn, 
            Card? lastPlayedCard,
            IEnumerable<WeatherCard> weatherCards)
        {
            Player = player;
            Opponent = opponent;
            GameScore = gameScore;
            GameStatus = gameStatus;
            IsPlayerTurn = isPlayerTurn;
            LastPlayedCard = lastPlayedCard;
            WeatherCards = weatherCards;
        }
    }
}
