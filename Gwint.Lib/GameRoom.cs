namespace Gwint.Lib
{
    public class GameRoom(string id, string hostName) : IGameState
    {
        private readonly Dictionary<Type, int> _gameStates =
            new()
            {
                { typeof(OpponentJoiningState), 1 },
                { typeof(PlayersAcceptingState), 2 },
                { typeof(DeckSelectingState), 3 },
                { typeof(GameRunningState), 4 },
            };

        public string Id { get; } = id;
        public bool IsFree { get; set; } = true;

        public IGameState GameState { get; private set; } = new OpponentJoiningState(hostName);

        public Player Host => GameState.Host;

        public Player Opponent
        {
            get
            {
                if (GameState is OpponentJoiningState)
                {
                    return null;
                }

                return GameState.Opponent;
            }
        }

        public Player CurrentPlayerTurn
        {
            get
            {
                if (GameState is not GameRunningState)
                {
                    return null;
                }

                return GameState.CurrentPlayerTurn;
            }
        }

        public Card? LastPlayedCard
        {
            get
            {
                if (GameState is not GameRunningState)
                {
                    return null;
                }
                
                return GameState.LastPlayedCard;
            }
        }

        public GameStatus GameStatus => GameState.GameStatus;

        public GameScore GameScore
        {
            get
            {
                if (GameState is not GameRunningState)
                {
                    return new GameScore();
                }

                return GameState.GameScore;
            }
        }



        public void SetGameState(IGameState newState)
        {
            if (!_gameStates.TryGetValue(newState.GetType(), out int newStateKey))
            {
                throw new ArgumentException(
                    $"Trying to set invalid game state of type: {newState.GetType()}"
                );
            }

            int currentStateKey = _gameStates[GameState.GetType()];

            if (newStateKey - currentStateKey != 1)
            {
                throw new InvalidOperationException(
                    $"Invalid game state transition. Current state: {GameState.GetType()}, next state: {newState.GetType()}"
                );
            }

            GameState = newState;
        }

        public bool AcceptGame(string playerId)
        {
            if (GameState is PlayersAcceptingState playersAcceptingState)
            {
                return playersAcceptingState.AcceptGame(playerId);
            }
            
            throw new InvalidOperationException("Can not accept game in current moment");
        }

        public bool SelectDeck(string playerId, DeckType deckType)
        {
            if (GameState is DeckSelectingState deckSelectingState)
            {
                return deckSelectingState.SelectDeck(playerId, deckType);
            }

            throw new InvalidOperationException("Can not select game in current moment");
        }

        public string? ProcessMove(string playerId, string? cardId, string? targetCardId)
        {
            if (GameState is GameRunningState runningState)
            {
                return runningState.ProcessMove(playerId, cardId, targetCardId);
            }

            throw new InvalidOperationException("Can not make move in current moment");
        }
    }
}
