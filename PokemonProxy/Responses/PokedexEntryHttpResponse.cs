using PokemonProxy.Services.Model;

namespace PokemonProxy.Responses;

public class PokedexEntryHttpResponse
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public string Habitat { get; set; }
    public bool Legendary { get; set; }

    public static PokedexEntryHttpResponse From(PokemonData data)
    {
        return new PokedexEntryHttpResponse
        {
            Name = data.Name,
            Description = data.Description,
            Habitat = data.Habitat,
            Legendary = data.Legendary
        };
    }
}