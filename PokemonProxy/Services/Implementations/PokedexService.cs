using System.Text.Json;
using System.Text.Json.Serialization;
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
            JsonSerializer.Deserialize(await pokemonSpeciesGetResponse.Content
                .ReadAsStringAsync(), SourceGenerationContext.Default.PokemonSpeciesGetResponseBody);
        if (pokemonSpeciesGetResponseBody == null)
            return null;

        return new PokemonData
        {
            Name = pokemonSpeciesGetResponseBody.Name,
            Habitat = pokemonSpeciesGetResponseBody.Habitat.Name,
            Legendary = pokemonSpeciesGetResponseBody.Legendary,
            Description = pokemonSpeciesGetResponseBody.FlavorTextEntries
                .FirstOrDefault(entry => entry.Language.Name == "en")?.FlavorText?.Replace("\n", " ").Replace("\f", " ")
        };
    }


    public async Task<PokemonData?> GetPokemonDataTranslatedAsync(string pokemonName)
    {
        var pokemonData = await GetPokemonDataAsync(pokemonName);
        if (pokemonData == null)
            return null;

        var httpClient = httpClientFactory.CreateClient("FunTranslationApi");
        var translatedPokemonDescriptionResponse = await httpClient.PostAsync(
            httpClient.BaseAddress + "/" +
            (pokemonData.Legendary || pokemonData.Habitat == "cave" ? "yoda" : "shakespeare") +
            ".json",
            JsonContent.Create(new FunTranslationApiRequest { Text = pokemonData.Description },
                SourceGenerationContext.Default.FunTranslationApiRequest));

        if (!translatedPokemonDescriptionResponse.IsSuccessStatusCode)
            return pokemonData;

        var translatedPokemonDescriptionResponseBody =
            JsonSerializer.Deserialize(await translatedPokemonDescriptionResponse.Content.ReadAsStringAsync(),
                SourceGenerationContext.Default.TranslatedPokemonDescriptionResponseBody);

        if (translatedPokemonDescriptionResponseBody == null)
            return pokemonData;

        pokemonData.Description = translatedPokemonDescriptionResponseBody.Contents.Translated;
        return pokemonData;
    }
}

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(TranslatedPokemonDescriptionResponseBody))]
[JsonSerializable(typeof(PokemonSpeciesGetResponseBody))]
[JsonSerializable(typeof(FunTranslationApiRequest))]
internal partial class SourceGenerationContext : JsonSerializerContext
{
}