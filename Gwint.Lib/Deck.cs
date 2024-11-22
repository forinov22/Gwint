using System.Collections.ObjectModel;

namespace Gwint.Lib
{
    public enum DeckType
    {
        Nilfgaardian,
        Northern,
    }

    public class Deck(List<Card> cards, DeckType deckType)
    {
        private readonly List<Card> _cards = cards.Skip(10).ToList();
        private readonly List<Card> _hand = cards.Take(10).ToList();
        private readonly List<UnitCard> _playedCards = [];
        private readonly List<UnitCard> _releasedCards = [];

        public DeckType DeckType { get; init; } = deckType;
        public ReadOnlyCollection<Card> Cards => _cards.AsReadOnly();
        public ReadOnlyCollection<Card> Hand => _hand.AsReadOnly();
        public ReadOnlyCollection<UnitCard> PlayedCards => _playedCards.AsReadOnly();
        public ReadOnlyCollection<UnitCard> ReleasedCards => _releasedCards.AsReadOnly();
        
        public Card? GetCardFromHandById(string cardId) =>
            _hand.FirstOrDefault(c => c.Id == cardId);

        public UnitCard? GetCardFromPlayedById(string cardId) =>
            _playedCards.FirstOrDefault(c => c.Id == cardId);

        public void PlayCard(Card card)
        {
            if (card is UnitCard unitCard)
            {
                AddCardToPlayed(unitCard);
            }

            RemoveCardFromHand(card);
        }

        public void ReleaseCard(UnitCard card)
        {
            AddCardToReleased(card);
            RemoveCardFromHand(card);
        }

        public void AddCardToPlayed(UnitCard card) => _playedCards.Add(card);

        public void RemoveCardFromPlayed(UnitCard card) => _playedCards.Remove(card);

        public void RemoveCardFromReleased(UnitCard card) => _releasedCards.Remove(card);

        public int CalculateScore() =>
            _playedCards.Aggregate(0, (total, card) => total + card.CurrentStrength);
        
        private void AddCardToReleased(UnitCard card) => _releasedCards.Add(card);

        private bool RemoveCardFromHand(Card card) => _hand.Remove(card);
    }
}
