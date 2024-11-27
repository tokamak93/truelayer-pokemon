using PokemonProxy.Services.Model;

namespace PokemonProxy.Services.Abstractions;

public interface IPokedexService
{
    Task<PokemonData?> GetPokemonDataAsync(string pokemonName);
    Task<PokemonData?> GetPokemonDataTranslatedAsync(string pokemonName);
}