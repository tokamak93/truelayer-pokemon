using System.Text.Json.Serialization;
using PokemonProxy.Responses;
using PokemonProxy.Services.Abstractions;
using PokemonProxy.Services.Implementations;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

builder.Services.AddSingleton<IPokedexService, PokedexService>();

// TODO
builder.Services.AddHttpClient();

var app = builder.Build();

app.MapGet("/pokemon/{pokemonName}", async (string pokemonName, IPokedexService pokedexService) =>
{
    var pokemonData = await pokedexService.GetPokemonDataAsync(pokemonName);
    if (pokemonData != null)
        Results.Ok(PokedexEntryHttpResponse.From(pokemonData));
    else
        Results.NotFound();
});

app.MapGet("/translated/pokemon/{pokemonName}", async (string pokemonName, IPokedexService pokedexService) =>
{
    var pokemonData = await pokedexService.GetPokemonDataTranslatedAsync(pokemonName);
    if (pokemonData != null)
        Results.Ok(PokedexEntryHttpResponse.From(pokemonData));
    else
        Results.NotFound();
});


[JsonSerializable(typeof(PokedexEntryHttpResponse[]))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{
}