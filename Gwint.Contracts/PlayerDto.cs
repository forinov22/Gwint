using System.Text.Json.Serialization;
using System.Xml.Linq;

using Gwint.Contracts;
using Gwint.Lib;

namespace Gwint.Api.DTOs
{
    public class PlayerDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public PlayerDeckDto? Deck { get; set; }

        public PlayerDto(string id, string name, PlayerDeckDto? deck)
        {
            Id = id;
            Name = name;
            Deck = deck;
        }
    }
}
