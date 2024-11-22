using System.Text.Json.Serialization;

using Gwint.Api.DTOs;

namespace Gwint.Contracts
{
    public class GameRoomDto
    {
        public string Id { get; set; }
        public bool IsFree { get; set; }
        public PlayerGameStateDto GameState { get; set; }

        public GameRoomDto(string id, bool isFree, PlayerGameStateDto gameState)
        {
            Id = id;
            IsFree = isFree;
            GameState = gameState;
        }
    };
}
