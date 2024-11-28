using System.Text.Json.Serialization;
using PokemonProxy.Responses;
using PokemonProxy.Services.Abstractions;
using PokemonProxy.Services.Implementations;
using PokemonProxy.Services.Model;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

builder.Services.AddSingleton<IPokedexService, PokedexService>();

builder.Services.AddHttpClient("PokeApi", client => { client.BaseAddress = new Uri("https://pokeapi.co/api/v2"); });
builder.Services.AddHttpClient("FunTranslationApi",
    client => { client.BaseAddress = new Uri("https://api.funtranslations.com/translate"); });

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