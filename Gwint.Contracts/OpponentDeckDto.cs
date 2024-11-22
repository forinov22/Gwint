using System.Text.Json.Serialization;

using Gwint.Lib;

namespace Gwint.Contracts
{
    public class OpponentDeckDto
    {
        public DeckType DeckType { get; set; }
        public int CardsInHand { get; set; }
        public List<UnitCard> PlayedCards { get; set; }
        public List<UnitCard> ReleasedCards { get; set; }

        public OpponentDeckDto(DeckType deckType, int cardsInHand, List<UnitCard> playedCards, List<UnitCard> releasedCards)
        {
            DeckType = deckType;
            CardsInHand = cardsInHand;
            PlayedCards = playedCards;
            ReleasedCards = releasedCards;
        }
    }
}
