using PokemonProxy.Services.Abstractions;
using PokemonProxy.Services.Model;

namespace PokemonProxy.Services.Implementations;

public class PokedexService : IPokedexService
{
    public Task<PokemonData?> GetPokemonDataAsync(string pokemonName)
    {
        throw new NotImplementedException();
    }

    public Task<PokemonData?> GetPokemonDataTranslatedAsync(string pokemonName)
    {
        throw new NotImplementedException();
    }
}