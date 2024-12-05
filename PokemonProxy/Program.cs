using System.Net;
using System.Text.Json.Serialization;
using PokemonProxy.Responses;
using PokemonProxy.Services.Abstractions;
using PokemonProxy.Services.Implementations;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

builder.Services.AddSingleton<IPokedexService, PokedexService>();

builder.Services.AddHttpClient("PokeApi", client => { client.BaseAddress = new Uri("https://pokeapi.co/api/v2"); });
builder.Services.AddHttpClient("FunTranslationApi",
    client => { client.BaseAddress = new Uri("https://api.funtranslations.com/translate"); });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
app.UseSwagger();
app.UseSwaggerUI();
app.MapSwagger();
app.Run();


[JsonSerializable(typeof(PokedexEntryHttpResponse[]))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{
}