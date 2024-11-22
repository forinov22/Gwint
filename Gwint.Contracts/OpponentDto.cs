using Gwint.Lib;

using System.Text.Json.Serialization;
using System.Xml.Linq;

namespace Gwint.Contracts
{
    public class OpponentDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public OpponentDeckDto? Deck { get; set; }

        public OpponentDto(string id, string name, OpponentDeckDto? deck)
        {
            Id = id;
            Name = name;
            Deck = deck;
        }
    }
}
