using System.Text.Json;
using PokemonProxy.Services.Abstractions;
using PokemonProxy.Services.Model;

namespace PokemonProxy.Services.Implementations;

public class PokedexService(IHttpClientFactory httpClientFactory) : IPokedexService
{
    public async Task<PokemonData?> GetPokemonDataAsync(string pokemonName)
    {
        var httpClient = httpClientFactory.CreateClient("PokeApi");

        var pokemonSpeciesGetResponse =
            await httpClient.GetAsync(httpClient.BaseAddress + $"/pokemon-species/{pokemonName}");
        if (!pokemonSpeciesGetResponse.IsSuccessStatusCode)
            return null;

        var pokemonSpeciesGetResponseBody =
            JsonSerializer.Deserialize<PokemonSpeciesGetResponseBody>(await pokemonSpeciesGetResponse.Content
                .ReadAsStringAsync());
        if (pokemonSpeciesGetResponseBody == null)
            return null;

        return new PokemonData
        {
            Name = pokemonSpeciesGetResponseBody.Name,
            Habitat = pokemonSpeciesGetResponseBody.Habitat.Name,
            Legendary = pokemonSpeciesGetResponseBody.Legendary,
            Description = pokemonSpeciesGetResponseBody.FlavorTextEntries
                .FirstOrDefault(entry => entry.Language.Name == "en")?.FlavorText
        };
    }


    public async Task<PokemonData?> GetPokemonDataTranslatedAsync(string pokemonName)
    {
        throw new NotImplementedException();
    }
}