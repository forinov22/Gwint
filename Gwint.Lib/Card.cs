using System.Text.Json.Serialization;

namespace Gwint.Lib
{
    [JsonPolymorphic]
    [JsonDerivedType(typeof(UnitCard), "unit")]
    [JsonDerivedType(typeof(WeatherCard), "weather")]
    public abstract class Card
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public string ImageUrl { get; set; }

        protected Card(string name, string imageUrl)
        {
            Name = name;
            ImageUrl = imageUrl;
        }
    }

    public enum WeatherType
    {
        Frost,
        Fog,
        Rain,
        Clear,
    }

    public sealed class WeatherCard : Card
    {
        public WeatherType WeatherType { get; }

        public WeatherCard(string name, string imageUrl, WeatherType weatherType) : base(name, imageUrl)
        {
            WeatherType = weatherType;
        }
    }

    public enum UnitType
    {
        Soldier,
        Hero,
        Healer,
        Spy,
    }

    public enum UnitRange
    {
        Melee,
        Ranged,
        Siege,
    }

    public sealed class UnitCard : Card
    {
        public int InitialStrength { get; }
        public int CurrentStrength { get; set; }
        public UnitType UnitType { get; }
        public UnitRange UnitRange { get; }

        public UnitCard(string name, string imageUrl, int initialStrength, UnitType unitType, UnitRange unitRange) : base(name, imageUrl)
        {
            InitialStrength = initialStrength;
            CurrentStrength = initialStrength;
            UnitType = unitType;
            UnitRange = unitRange;
        }

        public void ResetCardStrength()
        {
            CurrentStrength = InitialStrength;
        }
    }
}