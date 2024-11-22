using Gwint.Api.DTOs;
using Gwint.Lib;

namespace Gwint.Contracts
{
    public static class Mapper
    {
        public static GameScoreDto ToGameScoreDto(this GameScore gameScore, bool isHost)
        {
            return isHost
                ? new GameScoreDto()
                {
                    PlayerTotalScore = gameScore.HostTotalScore,
                    PlayerRoundWins = gameScore.HostRoundWins,
                    OpponentTotalScore = gameScore.OpponentTotalScore,
                    OpponentRoundWins = gameScore.OpponentRoundWins
                }
                : new GameScoreDto()
                {
                    PlayerTotalScore = gameScore.OpponentTotalScore,
                    PlayerRoundWins = gameScore.OpponentRoundWins,
                    OpponentTotalScore = gameScore.HostTotalScore,
                    OpponentRoundWins = gameScore.HostRoundWins
                };
        }
        
        public static PlayerDeckDto ToPlayerDeckDto(this Deck playerDeck)
        {
            return new PlayerDeckDto
            (
                playerDeck.DeckType,
                [.. playerDeck.Hand],
                [.. playerDeck.PlayedCards],
                [.. playerDeck.ReleasedCards]
            );
        }

        public static OpponentDeckDto ToOpponentDeckDto(this Deck opponentDeck)
        {
            return new OpponentDeckDto
            (
                opponentDeck.DeckType,
                opponentDeck.Hand.Count,
                [.. opponentDeck.PlayedCards],
                [.. opponentDeck.ReleasedCards]
            );
        }

        public static PlayerDto ToPlayerDto(this Player player)
        {
            return new PlayerDto
            (
                player.Id,
                player.Name,
                player.Deck?.ToPlayerDeckDto()
            );
        }

        public static OpponentDto ToOpponentDto(this Player opponent)
        {
            return new OpponentDto
            (
                opponent.Id,
                opponent.Name,
                opponent.Deck?.ToOpponentDeckDto()
            );
        }

        public static PlayerGameStateDto ToPlayerGameStateDto(this GameRoom room, bool isHost)
        {
            return isHost
                ? new PlayerGameStateDto
                (
                    room.Host.ToPlayerDto(),
                    room.Opponent?.ToOpponentDto(),
                    room.GameScore.ToGameScoreDto(isHost),
                    room.GameStatus,
                    room?.CurrentPlayerTurn is not null
                        && room.Host == room.CurrentPlayerTurn
                )
                : new PlayerGameStateDto
                (
                    room.Opponent.ToPlayerDto(),
                    room.Host.ToOpponentDto(),
                    room.GameScore.ToGameScoreDto(isHost),
                    room.GameStatus,
                    room?.CurrentPlayerTurn is not null
                        && room.Opponent == room.CurrentPlayerTurn
                );
        }

        public static GameRoomDto ToGameRoomDto(this GameRoom room, bool isHost)
        {
            return new GameRoomDto
            (
                room.Id,
                room.IsFree,
                room.ToPlayerGameStateDto(isHost)
            );
        }
    }
}
