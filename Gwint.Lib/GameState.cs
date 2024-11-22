namespace Gwint.Lib
{
    public enum GameStatus
    {
        Setup,
        InProcess,
        RoundEnded,
        GameEnded,
    }

    public struct GameScore
    {
        public int HostTotalScore { get; init; }
        public int OpponentTotalScore { get; init; }
        public int HostRoundWins { get; init; }
        public int OpponentRoundWins { get; init; }
    }

    public interface IGameState
    {
        Player Host { get; }
        Player Opponent { get; }
        Player CurrentPlayerTurn { get; }

        GameStatus GameStatus => GameStatus.Setup;
        GameScore GameScore { get; }

        bool AcceptGame(string playerId);
        bool SelectDeck(string playerId, DeckType deckType);
        string? ProcessMove(string playerId, string? cardId, string? targetCardId);
    }

    public class OpponentJoiningState(string hostName) : IGameState
    {
        public Player Host { get; } = new(hostName);
        public Player Opponent => throw new NotImplementedException();
        public Player CurrentPlayerTurn => throw new NotImplementedException();

        public GameScore GameScore => throw new NotImplementedException();

        public bool AcceptGame(string playerId)
        {
            throw new NotImplementedException();
        }

        public bool SelectDeck(string playerId, DeckType deckType)
        {
            throw new NotImplementedException();
        }

        public string? ProcessMove(string playerId, string? cardId, string? targetCardId)
        {
            throw new NotImplementedException();
        }
    }

    public class PlayersAcceptingState(Player host, Player opponent) : IGameState
    {
        private bool _hostAccepted;
        private bool _opponentAccepted;

        public Player Host { get; } = host;
        public Player Opponent { get; } = opponent;
        public Player CurrentPlayerTurn => throw new NotImplementedException();

        public GameScore GameScore => throw new NotImplementedException();

        public bool AcceptGame(string playerId)
        {
            if (Host.Id == playerId)
            {
                _hostAccepted = true;
            }
            else if (Opponent.Id == playerId)
            {
                _opponentAccepted = true;
            }
            else
            {
                throw new ArgumentException($"Player with id: {playerId} not exists in room");
            }

            return _hostAccepted && _opponentAccepted;
        }

        public bool SelectDeck(string playerId, DeckType deckType)
        {
            throw new NotImplementedException();
        }

        public string? ProcessMove(string playerId, string? cardId, string? targetCardId)
        {
            throw new NotImplementedException();
        }
    }

    public class DeckSelectingState(Player host, Player opponent) : IGameState
    {
        private bool _hostSelected;
        private bool _opponentSelected;

        public Player Host { get; } = host;
        public Player Opponent { get; } = opponent;
        public Player CurrentPlayerTurn => throw new NotImplementedException();

        public GameScore GameScore => throw new NotImplementedException();

        public bool AcceptGame(string playerId)
        {
            throw new NotImplementedException();
        }

        public bool SelectDeck(string playerId, DeckType deckType)
        {
            if (Host.Id == playerId)
            {
                Host.SetDeck(deckType);
                _hostSelected = true;
            }
            else if (Opponent.Id == playerId)
            {
                Opponent.SetDeck(deckType);
                _opponentSelected = true;
            }
            else
            {
                throw new ArgumentException($"Player with id: {playerId} not exists in room");
            }

            return _hostSelected && _opponentSelected;
        }

        public string? ProcessMove(string playerId, string? cardId, string? targetCardId)
        {
            throw new NotImplementedException();
        }
    }

    public class GameRunningState(Player host, Player opponent) : IGameState
    {
        private bool _hostSkipped;
        private bool _opponentSkipped;

        public Player Host { get; } = host;
        public Player Opponent { get; } = opponent;
        public Player CurrentPlayerTurn { get; private set; } = host;

        public GameStatus GameStatus { get; private set; } = GameStatus.InProcess;
        public GameScore GameScore { get; private set; }

        public bool AcceptGame(string playerId)
        {
            throw new NotImplementedException();
        }

        public bool SelectDeck(string playerId, DeckType deckType)
        {
            throw new NotImplementedException();
        }
        
        public string? ProcessMove(string playerId, string? cardId, string? targetCardId)
        {
            GameStatus = GameStatus.InProcess;

            ValidatePlayerTurn(playerId);

            if (IsTurnSkipped())
            {
                SwitchPlayerTurn();
                return null;
            }

            if (cardId is null)
            {
                HandleSkipTurn(playerId);
                SwitchPlayerTurn();
                return (_hostSkipped && _opponentSkipped) ? EndRound() : null;
            }

            Card card = CurrentPlayerTurn.PlayCard(cardId) 
                        ?? throw new ArgumentException($"User does not have card with id: {cardId} in their hand");

            switch (card)
            {
                case UnitCard unitCard:
                    HandleUnitCard(unitCard, targetCardId);
                    break;
                case WeatherCard weatherCard:
                    HandleWeatherCard(weatherCard);
                    break;
                default:
                    throw new InvalidOperationException("Unsupported card type.");
            }

            UpdateScores();

            SwitchPlayerTurn();

            return null;
        }

        private void UpdateScores()
        {
            GameScore = GameScore with
            {
                HostTotalScore = Host.GetCurrentScore(),
                OpponentTotalScore = Opponent.GetCurrentScore()
            };
        }
        
        private void ValidatePlayerTurn(string playerId)
        {
            if (CurrentPlayerTurn.Id != playerId)
            {
                throw new InvalidOperationException($"Can't make move. Current player turn: {CurrentPlayerTurn.Id}");
            }
        }

        private void HandleSkipTurn(string playerId)
        {
            if (playerId == Host.Id)
                _hostSkipped = true;
            else
                _opponentSkipped = true;
        }

        private void HandleUnitCard(UnitCard unitCard, string? targetCardId)
        {
            switch (unitCard.UnitType)
            {
                case UnitType.Healer:
                    HealCard(targetCardId);
                    break;
                case UnitType.Spy:
                    SwapCardsWithOpponent(unitCard, targetCardId);
                    break;
            }
        }

        private void HealCard(string? targetCardId)
        {
            if (targetCardId is null)
                throw new ArgumentException("Target card ID cannot be null for healing.");

            var targetCard = CurrentPlayerTurn.Deck.ReleasedCards
                                 .FirstOrDefault(c => c.Id == targetCardId) 
                             ?? throw new ArgumentException($"No released card found with ID: {targetCardId}");

            CurrentPlayerTurn.HealCard(targetCard);
        }

        private void SwapCardsWithOpponent(UnitCard unitCard, string? targetCardId)
        {
            if (targetCardId is null)
                throw new ArgumentException("Target card ID cannot be null for swapping.");

            var opponent = GetOpponent();

            CurrentPlayerTurn.Deck.RemoveCardFromPlayed(unitCard);
            opponent.Deck.AddCardToPlayed(unitCard);

            var targetCard = opponent.Deck.GetCardFromPlayedById(targetCardId) 
                             ?? throw new ArgumentException($"No played card found with ID: {targetCardId}");

            opponent.Deck.RemoveCardFromPlayed(targetCard);
            CurrentPlayerTurn.Deck.AddCardToPlayed(targetCard);
        }

        private void HandleWeatherCard(WeatherCard weatherCard)
        {
            var opponent = GetOpponent();

            IEnumerable<UnitCard> AffectedCards(IEnumerable<UnitCard> cards, UnitRange range) =>
                cards.Where(c => c.UnitRange == range && c.UnitType != UnitType.Hero);

            switch (weatherCard.WeatherType)
            {
                case WeatherType.Frost:
                    ApplyWeatherEffect(AffectedCards(CurrentPlayerTurn.Deck.PlayedCards, UnitRange.Melee), 1);
                    ApplyWeatherEffect(AffectedCards(opponent.Deck.PlayedCards, UnitRange.Melee), 1);
                    break;
                case WeatherType.Fog:
                    ApplyWeatherEffect(AffectedCards(CurrentPlayerTurn.Deck.PlayedCards, UnitRange.Ranged), 1);
                    ApplyWeatherEffect(AffectedCards(opponent.Deck.PlayedCards, UnitRange.Ranged), 1);
                    break;
                case WeatherType.Rain:
                    ApplyWeatherEffect(AffectedCards(CurrentPlayerTurn.Deck.PlayedCards, UnitRange.Siege), 1);
                    ApplyWeatherEffect(AffectedCards(opponent.Deck.PlayedCards, UnitRange.Siege), 1);
                    break;
                case WeatherType.Clear:
                    ResetWeatherEffects(CurrentPlayerTurn.Deck.PlayedCards);
                    ResetWeatherEffects(opponent.Deck.PlayedCards);
                    break;
            }
        }

        private void ApplyWeatherEffect(IEnumerable<UnitCard> cards, int strength)
        {
            foreach (var card in cards)
            {
                card.CurrentStrength = strength;
            }
        }

        private void ResetWeatherEffects(IEnumerable<UnitCard> cards)
        {
            foreach (var card in cards.Where(c => c.UnitType != UnitType.Hero))
            {
                card.CurrentStrength = card.InitialStrength;
            }
        }

        private string EndRound()
        {
            string roundWinner;
            
            if (GameScore.HostTotalScore > GameScore.OpponentTotalScore)
            {
                GameScore = GameScore with { HostRoundWins = GameScore.HostRoundWins + 1 };
                roundWinner = Host.Name;
            }
            else if (GameScore.HostTotalScore < GameScore.OpponentTotalScore)
            {
                GameScore = GameScore with { OpponentRoundWins = GameScore.OpponentRoundWins + 1 };
                roundWinner = Opponent.Name;
            }
            else
            {
                GameScore = GameScore with
                {
                    HostRoundWins = GameScore.HostRoundWins + 1, OpponentRoundWins = GameScore.OpponentRoundWins + 1
                };
                roundWinner = "Draw";
            }

            GameScore = GameScore with { HostTotalScore = 0, OpponentTotalScore = 0 };
            _hostSkipped = false;
            _opponentSkipped = false;

            if (GameScore.HostRoundWins == 2 || GameScore.OpponentRoundWins == 2)
            {   
                GameStatus = GameStatus.GameEnded;
                return roundWinner;
            }

            foreach (UnitCard playedCard in Host.Deck.PlayedCards.ToList())
            {
                playedCard.ResetCardStrength();

                _ = Host.ReleaseCard(playedCard.Id);
                Host.Deck.RemoveCardFromPlayed(playedCard);
            }

            foreach (UnitCard playedCard in Opponent.Deck.PlayedCards.ToList())
            {
                playedCard.ResetCardStrength();

                _ = Opponent.ReleaseCard(playedCard.Id);
                Opponent.Deck.RemoveCardFromPlayed(playedCard);
            }

            GameStatus = GameStatus.RoundEnded;

            return roundWinner;
        }

        private Player GetOpponent()
        {
            return CurrentPlayerTurn.Id == Host.Id
                ? Opponent
                : Host;
        }
        
        private bool IsTurnSkipped()
        {
            return (CurrentPlayerTurn.Id == Host.Id && _hostSkipped)
                   || (CurrentPlayerTurn.Id == Opponent.Id && _opponentSkipped);
        }

        private void SwitchPlayerTurn()
        {
            CurrentPlayerTurn = CurrentPlayerTurn.Id == Host.Id ? Opponent : Host;
        }
    }
}