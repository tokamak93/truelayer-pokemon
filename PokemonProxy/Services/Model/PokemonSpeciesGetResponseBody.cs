using System.Text.Json.Serialization;

namespace PokemonProxy.Services.Model;

public class PokemonSpeciesGetResponseBody
{
    [JsonPropertyName("flavor_text_entries")]
    public FlavorTextEntries[] FlavorTextEntries { get; set; }

    [JsonPropertyName("name")] public string Name { get; set; }
    [JsonPropertyName("is_legendary")] public bool Legendary { get; set; }
    [JsonPropertyName("habitat")] public Habitat Habitat { get; set; }
}

public class FlavorTextEntries
{
    [JsonPropertyName("flavor_text")] public string? FlavorText { get; set; }

    [JsonPropertyName("language")] public Language Language { get; set; }
}

public class Language
{
    [JsonPropertyName("name")] public string Name { get; set; }
}

public class Habitat
{
    [JsonPropertyName("name")] public string Name { get; set; }
}