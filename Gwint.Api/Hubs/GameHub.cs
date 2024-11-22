using System.Collections.Concurrent;
using System.Text.Json;

using Gwint.Api.DTOs;
using Gwint.Contracts;
using Gwint.Lib;

using Microsoft.AspNetCore.SignalR;

namespace Gwint.Api.Hubs
{
    public interface IGameClient
    {
        Task RoomsUpdated(List<GameRoomDto> rooms);
        Task OpponentJoined(OpponentDto opponent);
        Task OpponentAccepted();
        Task DeckSelectStarted();
        Task OpponentSelectedDeck(DeckType deckType);
        Task GameStarted(PlayerGameStateDto state);
        Task PlayerMoved(PlayerGameStateDto state);
        Task RoundEnded(RoundEndedDto dto);
        Task GameEnded(GameEndedDto dto);
    }

    public class GameHub(ILogger<GameHub> logger) : Hub<IGameClient>
    {
        private static readonly ConcurrentDictionary<string, GameRoom> GameRooms = new();

        public List<GameRoomDto> GetAllFreeRooms()
        {
            List<GameRoomDto> rooms = GameRooms
                .Where(kvp => kvp.Value.IsFree)
                .Select(kvp => kvp.Value.ToGameRoomDto(true))
                .ToList();

            return rooms;
        }

        public async Task<GameRoomDto> CreateRoom(string playerName)
        {
            string roomId = Guid.NewGuid().ToString();
            GameRoom room = new(roomId, playerName);
            GameRooms[roomId] = room;

            await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
            await Clients.All.RoomsUpdated(GetAllFreeRooms());

            return room.ToGameRoomDto(true);
        }

        public async Task<GameRoomDto> JoinRoom(string roomId, string playerName)
        {
            GameRoom room = GetRoomById(roomId);

            room.SetGameState(
                new PlayersAcceptingState(room.GameState.Host, new Player(playerName))
            );

            room.IsFree = false;

            await Groups.AddToGroupAsync(Context.ConnectionId, roomId);

            await Clients.OthersInGroup(roomId).OpponentJoined(room.Opponent.ToOpponentDto());

            return room.ToGameRoomDto(false);
        }

        public async Task AcceptGame(string roomId, string playerId)
        {
            GameRoom room = GetRoomById(roomId);

            bool bothPlayersReady = room.AcceptGame(playerId);

            await Clients.OthersInGroup(roomId).OpponentAccepted();

            if (bothPlayersReady)
            {
                room.SetGameState(
                    new DeckSelectingState(room.GameState.Host, room.GameState.Opponent)
                );

                await Clients.Groups(roomId).DeckSelectStarted();
            }
        }

        public async Task SelectDeck(string roomId, string playerId, DeckType deckType)
        {
            GameRoom room = GetRoomById(roomId);

            bool bothPlayerSelectedDeck = room.SelectDeck(playerId, deckType);

            await Clients.OthersInGroup(roomId).OpponentSelectedDeck(deckType);
            
            if (bothPlayerSelectedDeck)
            {
                room.SetGameState(new GameRunningState(room.GameState.Host, room.GameState.Opponent));

                GetPlayerAndOpponentGameStates(playerId, room, out PlayerGameStateDto playerGameState, out PlayerGameStateDto opponentGameState);

                await Clients.Caller.GameStarted(playerGameState);
                await Clients.OthersInGroup(roomId).GameStarted(opponentGameState);
            }
        }

        public async Task MakeMove(string roomId, string playerId, string? cardId, string? targetCardId)
        {
            GameRoom room = GetRoomById(roomId);

            var winner = room.ProcessMove(playerId, cardId, targetCardId);

            GetPlayerAndOpponentGameStates(playerId, room, out PlayerGameStateDto playerGameState, out PlayerGameStateDto opponentGameState);

            await Clients.Caller.PlayerMoved(playerGameState);
            await Clients.OthersInGroup(roomId).PlayerMoved(opponentGameState);

            switch (room.GameStatus)
            {
                case GameStatus.RoundEnded:
                    await RoundEnded(roomId, playerId, room, winner!);
                    break;
                case GameStatus.GameEnded:
                    await GameEnded(roomId, playerId, room, winner!);
                    break;
            }
        }

        private async Task RoundEnded(string roomId, string playerId, GameRoom room, string winner)
        {
            GetPlayerAndOpponentGameStates(playerId, room, out PlayerGameStateDto playerGameState, out PlayerGameStateDto opponentGameState);

            await Clients.Caller.RoundEnded(new RoundEndedDto(playerGameState, winner));
            await Clients.OthersInGroup(roomId).RoundEnded(new RoundEndedDto(opponentGameState, winner));
        }

        private async Task GameEnded(string roomId, string playerId, GameRoom room, string winner)
        {
            GetPlayerAndOpponentGameStates(playerId, room, out PlayerGameStateDto playerGameState, out PlayerGameStateDto opponentGameState);

            await Clients.Caller.GameEnded(new GameEndedDto(winner));
            await Clients.OthersInGroup(roomId).GameEnded(new GameEndedDto(winner));
        }

        private static GameRoom GetRoomById(string roomId)
        {
            if (!GameRooms.TryGetValue(roomId, out GameRoom? room))
            {
                throw new ArgumentException($"Room with id: {roomId} was not found");
            }
            return room;
        }

        private static void GetPlayerAndOpponentGameStates(string playerId, GameRoom room, out PlayerGameStateDto playerGameState, out PlayerGameStateDto opponentGameState)
        {
            if (room.Host.Id == playerId)
            {
                playerGameState = room.ToPlayerGameStateDto(true);
                opponentGameState = room.ToPlayerGameStateDto(false);
            }
            else
            {
                playerGameState = room.ToPlayerGameStateDto(false);
                opponentGameState = room.ToPlayerGameStateDto(true);
            }
        }
    }
}
