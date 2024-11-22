using System.Text.Json.Serialization;

using Gwint.Lib;

namespace Gwint.Contracts
{
    public class PlayerDeckDto
    {
        public DeckType DeckType { get; set; }
        public List<Card> Hand { get; set; }
        public List<UnitCard> PlayedCards { get; set; }
        public List<UnitCard> ReleasedCards { get; set; }

        public PlayerDeckDto(
            DeckType deckType,
            List<Card> hand,
            List<UnitCard> playedCards,
            List<UnitCard> releasedCards)
        {
            DeckType = deckType;
            Hand = hand;
            PlayedCards = playedCards;
            ReleasedCards = releasedCards;
        }
    }
}
