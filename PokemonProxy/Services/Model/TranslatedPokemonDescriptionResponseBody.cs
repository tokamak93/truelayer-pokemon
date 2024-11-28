using System.Text.Json.Serialization;

namespace PokemonProxy.Services.Model;

public class TranslatedPokemonDescriptionResponseBody
{
    [JsonPropertyName("contents")] public Contents Contents { get; set; }
}

public class Contents
{
    [JsonPropertyName("translated")] public string Translated { get; set; }
}