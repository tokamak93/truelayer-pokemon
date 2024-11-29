using System.Net;
using System.Text.Json.Serialization;
using PokemonProxy.Responses;
using PokemonProxy.Services.Abstractions;
using PokemonProxy.Services.Implementations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

builder.Services.AddSingleton<IPokedexService, PokedexService>();

builder.Services.AddHttpClient("PokeApi", client => { client.BaseAddress = new Uri("https://pokeapi.co/api/v2"); });
builder.Services.AddHttpClient("FunTranslationApi",
    client => { client.BaseAddress = new Uri("https://api.funtranslations.com/translate"); });

// builder.WebHost.ConfigureKestrel(options => options.ListenLocalhost(8080));

var app = builder.Build();

app.MapGet("/pokemon/{pokemonName}", async (string pokemonName, IPokedexService pokedexService) =>
{
    var pokemonData = await pokedexService.GetPokemonDataAsync(pokemonName);
    if (pokemonData != null)
        return Results.Ok(PokedexEntryHttpResponse.From(pokemonData));
    else
        return Results.NotFound();
});

app.MapGet("/translated/pokemon/{pokemonName}", async (string pokemonName, IPokedexService pokedexService) =>
{
    var pokemonData = await pokedexService.GetPokemonDataTranslatedAsync(pokemonName);
    if (pokemonData != null)
        return Results.Ok(PokedexEntryHttpResponse.From(pokemonData));
    else
        return Results.NotFound();
});

app.Run();


[JsonSerializable(typeof(PokedexEntryHttpResponse[]))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{
}