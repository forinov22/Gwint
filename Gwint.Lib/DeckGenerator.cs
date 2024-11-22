namespace Gwint.Lib
{
    public class DeckGenerator
    {
        private static readonly Random Rand = new();

        private static readonly List<UnitCard> NilfgaardianCards =
        [
            new UnitCard("Emhyr var Emreis", "/card-images/anton.png", 10, UnitType.Hero, UnitRange.Melee),
            new UnitCard("Morvran Voorhis", "/card-images/egor.png", 9, UnitType.Hero, UnitRange.Melee),
            new UnitCard("Letho of Gulet", "/card-images/paul.png", 10, UnitType.Hero, UnitRange.Melee),
            new UnitCard("Menno Coehoorn", "/card-images/rostik.png", 7, UnitType.Soldier, UnitRange.Ranged),
            new UnitCard("Stefan Skellen", "/card-images/shyshov.png", 9, UnitType.Soldier, UnitRange.Siege),
            new UnitCard("Joachim de Wett", "/card-images/stas.png", 5, UnitType.Healer, UnitRange.Ranged),
            new UnitCard("Vattier de Rideaux", "/card-images/ya.png", 7, UnitType.Spy, UnitRange.Siege),
            new UnitCard("Fringilla Vigo", "/card-images/yarik.png", 6, UnitType.Soldier, UnitRange.Ranged),
            new UnitCard("Albrich", "/card-images/marchenok.png", 4, UnitType.Soldier, UnitRange.Melee),
            new UnitCard("Cahir Mawr Dyffryn", "/card-images/sergeev.png", 6, UnitType.Soldier, UnitRange.Ranged),
        ];

        private static readonly List<UnitCard> NorthernCards =
        [
            new UnitCard("Foltest", "/card-images/kalugina.png", 10, UnitType.Hero, UnitRange.Melee),
            new UnitCard("Ciri", "/card-images/kvacha.png", 15, UnitType.Hero, UnitRange.Melee),
            new UnitCard("Yennefer of Vengerberg", "/card-images/malina.png", 10, UnitType.Hero, UnitRange.Siege),
            new UnitCard("Vernon Roche", "/card-images/markov.png", 8, UnitType.Spy, UnitRange.Ranged),
            new UnitCard("John Natalis", "/card-images/miganiga.png", 7, UnitType.Soldier, UnitRange.Siege),
            new UnitCard("Yarpen Zigrin", "/card-images/nester.png", 5, UnitType.Soldier, UnitRange.Melee),
            new UnitCard("Thaler", "/card-images/primicheva.png", 1, UnitType.Spy, UnitRange.Ranged),
            new UnitCard("Shani", "/card-images/snarski.png", 6, UnitType.Healer, UnitRange.Ranged),
            new UnitCard("Keira Metz", "/card-images/vadymtsev.png", 7, UnitType.Soldier, UnitRange.Siege),
            new UnitCard("Dethmold", "/card-images/ekonomika.png", 6, UnitType.Soldier, UnitRange.Ranged),
            new UnitCard("Dijkstra", "/card-images/sergeev.png", 4, UnitType.Spy, UnitRange.Siege)
        ];

        private static readonly List<WeatherCard> WeatherCards =
        [
            new WeatherCard("Biting Frost", "/card-images/frost.png", WeatherType.Frost),
            new WeatherCard("Impenetrable Fog", "/card-images/fog.png", WeatherType.Fog),
            new WeatherCard("Torrential Rain", "/card-images/rain.png", WeatherType.Rain),
            new WeatherCard("Clear Skies", "/card-images/clear.png", WeatherType.Clear)
        ];

        public static List<Card> GenerateDeck(DeckType deckType)
        {
            List<Card> result = [];
            
            if (deckType == DeckType.Nilfgaardian)
            {
                result.Add(new WeatherCard("Biting Frost", "/card-images/frost.png", WeatherType.Frost));
                result.AddRange(NilfgaardianCards);
                
            }
            else
            {
                result.Add(new WeatherCard("Torrential Rain", "/card-images/rain.png", WeatherType.Rain));
                result.AddRange(NorthernCards);
            }
            
            return result;
        }
        
        // public static List<Card> GenerateDeck(DeckType deckType)
        // {
        //     List<Card> selectedDeck = new();
        //     List<UnitCard> availableUnitCards = deckType == DeckType.Nilfgaardian ? NilfgaardianCards : NorthernCards;
        //
        //     // Track added hero cards to prevent duplicates
        //     var addedHeroCards = new HashSet<string>();
        //
        //     // Create a combined list of all card types
        //     var allCards = new List<Card>(availableUnitCards);
        //     allCards.AddRange(WeatherCards);
        //
        //     // Dictionary to track occurrences of non-hero cards
        //     var cardCounts = new Dictionary<Card, int>();
        //
        //     while (selectedDeck.Count < 25)
        //     {
        //         var card = allCards[Rand.Next(allCards.Count)];
        //
        //         if (card is UnitCard unitCard && unitCard.UnitType == UnitType.Hero)
        //         {
        //             // Add hero card if not already added
        //             if (!addedHeroCards.Contains(unitCard.Name))
        //             {
        //                 selectedDeck.Add(GenerateUniqueCard(unitCard));
        //                 addedHeroCards.Add(unitCard.Name);
        //             }
        //         }
        //         else
        //         {
        //             // For non-hero cards, allow up to 3 duplicates
        //             if (!cardCounts.TryGetValue(card, out int count))
        //             {
        //                 count = 0;
        //             }
        //
        //             if (count < 3)
        //             {
        //                 selectedDeck.Add(GenerateUniqueCard(card));
        //                 cardCounts[card] = count + 1;
        //             }
        //         }
        //     }
        //
        //     return selectedDeck;
        // }

        private static Card GenerateUniqueCard(Card card)
        {
            var uniqueId = Guid.NewGuid().ToString();
            return card switch
            {
                UnitCard unitCard => new UnitCard(unitCard.Name, unitCard.ImageUrl, unitCard.InitialStrength, unitCard.UnitType, unitCard.UnitRange),
                WeatherCard weatherCard => new WeatherCard(weatherCard.Name, weatherCard.ImageUrl, weatherCard.WeatherType),
                _ => throw new InvalidOperationException("Unknown card type.")
            };
        }
    }
}