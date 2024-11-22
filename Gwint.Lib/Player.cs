namespace Gwint.Lib
{
    public class Player(string name)
    {
        private bool IsReady { get; set; }
        
        public string Id { get; } = Guid.NewGuid().ToString();
        public string Name { get; } = name;
        public Deck Deck { get; private set; } = null!;

        public void SetDeck(DeckType deckType)
        {
            Deck = new Deck(DeckGenerator.GenerateDeck(deckType), deckType);
            IsReady = true;
        }

        public Card? PlayCard(string cardId)
        {
            ThrowIfNotReady();

            Card? playedCard = Deck.GetCardFromHandById(cardId);

            if (playedCard is not null)
            {
                Deck.PlayCard(playedCard);
            }

            return playedCard;
        }

        public Card? ReleaseCard(string cardId)
        {
            ThrowIfNotReady();

            UnitCard? releasedCard = Deck.GetCardFromPlayedById(cardId);

            if (releasedCard is not null)
            {
                Deck.ReleaseCard(releasedCard);
            }

            return releasedCard;
        }

        public void HealCard(UnitCard card)
        {
            Deck.RemoveCardFromReleased(card);
            Deck.AddCardToPlayed(card);
        }

        public int GetCurrentScore()
        {
            return Deck.CalculateScore();
        }

        private void ThrowIfNotReady()
        {
            if (!IsReady)
            {
                throw new InvalidOperationException("Deck should be selected before playing.");
            }
        }
    }
}
