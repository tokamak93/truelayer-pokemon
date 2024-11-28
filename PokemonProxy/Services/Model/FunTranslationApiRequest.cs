using System.Text.Json.Serialization;

namespace PokemonProxy.Services.Model;

public class FunTranslationApiRequest
{
    [JsonPropertyName("text")] public string? Text { get; set; }
}