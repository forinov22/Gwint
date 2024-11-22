using System.Text.Json;
using System.Text.Json.Serialization;

using Gwint.Api.DTOs;
using Gwint.Contracts;
using Gwint.Lib;

using Microsoft.AspNetCore.SignalR.Client;

namespace Gwint.Client.Services
{
    public class GameRoomService
    {
        private bool _isConnected;
        
        private readonly HubConnection _hubConnection;
        private readonly GameRoomState _gameRoomState;

        public event Action<List<GameRoomDto>>? OnRoomsUpdated;
        public event Action<OpponentDto>? OnOpponentJoined;
        public event Action? OnOpponentAccepted;
        public event Action? OnDeckSelectStarted;
        public event Action<DeckType>? OnOpponentSelectedDeck;
        public event Action<PlayerGameStateDto>? OnGameStarted;
        public event Action<PlayerGameStateDto>? OnPlayerMoved;
        public event Action<RoundEndedDto>? OnRoundEnded;
        public event Action<GameEndedDto>? OnGameEnded;

        public GameRoomService(IConfiguration configuration, GameRoomState gameRoomState)
        {
            string apiUrl = configuration["ApiUrl"] 
                            ?? throw new ArgumentException("Api url is not provided");

            _hubConnection = new HubConnectionBuilder()
                .WithUrl($"{apiUrl}/gameHub")
                .WithAutomaticReconnect()
                .Build();

            _hubConnection.On<List<GameRoomDto>>("RoomsUpdated", rooms => OnRoomsUpdated?.Invoke(rooms));
            _hubConnection.On<OpponentDto>("OpponentJoined", opponent => OnOpponentJoined?.Invoke(opponent));
            _hubConnection.On("OpponentAccepted", () => OnOpponentAccepted?.Invoke());
            _hubConnection.On("DeckSelectStarted", () => OnDeckSelectStarted?.Invoke());
            _hubConnection.On<DeckType>("OpponentSelectedDeck", deckType => OnOpponentSelectedDeck?.Invoke(deckType));
            _hubConnection.On<PlayerGameStateDto>("GameStarted", state => OnGameStarted?.Invoke(state));
            _hubConnection.On<PlayerGameStateDto>("PlayerMoved", state => OnPlayerMoved?.Invoke(state));
            _hubConnection.On<RoundEndedDto>("RoundEnded", dto => OnRoundEnded?.Invoke(dto));
            _hubConnection.On<GameEndedDto>("GameEnded", dto => OnGameEnded?.Invoke(dto));

            _gameRoomState = gameRoomState;
        }

        public async ValueTask StartAsync()
        {
            if (!_isConnected)
            {
                await _hubConnection.StartAsync();
                _isConnected = true;
            }
        }

        public async Task<List<GameRoomDto>> GetAllRooms()
        {
            return await _hubConnection.InvokeAsync<List<GameRoomDto>>("GetAllFreeRooms");
        }

        public async Task<GameRoomDto> CreateRoom(string userName)
        {
            var room = await _hubConnection.InvokeAsync<GameRoomDto>("CreateRoom", userName);

            _gameRoomState.SetRoom(room);

            return room;
        }

        public async Task<GameRoomDto> JoinRoom(string roomId, string userName)
        {
            var room = await _hubConnection.InvokeAsync<GameRoomDto>("JoinRoom", roomId, userName);

            _gameRoomState.SetRoom(room);

            return room;
        }

        public async Task AcceptGame(string roomId, string playerId)
        {
            await _hubConnection.InvokeAsync("AcceptGame", roomId, playerId);
        }

        public async Task SelectDeck(string roomId, string playerId, DeckType deckType)
        {
            await _hubConnection.InvokeAsync("SelectDeck", roomId, playerId, deckType);
        }

        public async Task MakeMove(string roomId, string playerId, string? cardId, string? targetCardId)
        {
            await _hubConnection.InvokeAsync("MakeMove", roomId, playerId, cardId, targetCardId);
        }

        public async ValueTask DisposeAsync()
        {
            //if (_isConnected)
            //{
            //    await _hubConnection.DisposeAsync();
            //    _isConnected = false;
            //}
        }
    }
}
