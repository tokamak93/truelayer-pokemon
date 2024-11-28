using System.Net;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Moq.Protected;
using PokemonProxy.Services.Abstractions;
using PokemonProxy.Services.Implementations;
using PokemonProxy.Services.Model;

namespace PokemonProxy.Test;

public class PokedexServiceIntegrationTest(IPokedexService sut)
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IPokedexService, PokedexService>();
            services.AddHttpClient("PokeApi", client => { client.BaseAddress = new Uri("https://pokeapi.co/api/v2"); });
            services.AddHttpClient("FunTranslationApi",
                client => { client.BaseAddress = new Uri("https://api.funtranslations.com/translate"); });
        }
    }

    [Fact]
    public async Task GetPokemonDataAsync_ShouldReturnPokemonData_WhenPokemonDataIsNotNull()
    {
        var pokemonData = await sut.GetPokemonDataAsync("mewtwo");

        Assert.NotNull(pokemonData);
        Assert.Equal("mewtwo", pokemonData.Name);
        Assert.True(pokemonData.Legendary);
        Assert.Equal("rare", pokemonData.Habitat);
        Assert.Equal(
            "It was created by\na scientist after\nyears of horrific\fgene splicing and\nDNA engineering\nexperiments.",
            pokemonData.Description);
    }

    [Fact]
    public async Task GetPokemonDataTranslatedAsync_ShouldReturnPokemonData_WhenPokemonDataIsNotNull()
    {
        var pokemonData = await sut.GetPokemonDataTranslatedAsync("mewtwo");

        Assert.NotNull(pokemonData);
        Assert.Equal("mewtwo", pokemonData.Name);
        Assert.True(pokemonData.Legendary);
        Assert.Equal("rare", pokemonData.Habitat);
        Assert.Equal(
            "Created by a scientist after years of horrific gene splicing and dna engineering experiments,  it was.",
            pokemonData.Description);
    }

    [Fact]
    public async Task GetPokemonDataAsync_ShouldReturnNull_WhenPokemonDataIsNull()
    {
        var pokemonData = await sut.GetPokemonDataAsync("mewtwoooo");
        Assert.Null(pokemonData);
    }
    [Fact]
    public async Task GetPokemonDataTranslatedAsync_ShouldReturnNull_WhenPokemonDataIsNull()
    {
        var pokemonData = await sut.GetPokemonDataTranslatedAsync("mewtwoooo");
        Assert.Null(pokemonData);
    }
}