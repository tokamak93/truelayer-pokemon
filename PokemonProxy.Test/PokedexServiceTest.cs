using System.Net;
using System.Text.Json;
using Moq;
using Moq.Protected;
using PokemonProxy.Services.Implementations;
using PokemonProxy.Services.Model;

namespace PokemonProxy.Test;

public class PokedexServiceTest
{
    [Fact]
    public async Task GetPokemonDataAsync_ShouldReturnPokemonData_WhenPokemonDataIsNotNull()
    {
        //setup proxy 
        var httpClientFactory = new Mock<IHttpClientFactory>();
        var httpResponse = new HttpResponseMessage(HttpStatusCode.Accepted)
        {
            Content = new StringContent(JsonSerializer.Serialize(new PokemonSpeciesGetResponseBody
            {
                Habitat = new Habitat
                {
                    Name = "rare"
                },
                Name = "mewtwo",
                Legendary = true,
                FlavorTextEntries =
                [
                    new FlavorTextEntries
                    {
                        FlavorText =
                            "Creato da uno scienziato dopo anni di orribili\nesperimenti di ingegneria genetica.",
                        Language = new Language { Name = "it" }
                    },
                    new FlavorTextEntries
                    {
                        FlavorText =
                            "It was created by\\na scientist after\\nyears of horrific\\fgene splicing and\\nDNA engineering\\nexperiments.",
                        Language = new Language { Name = "en" }
                    }
                ]
            }))
        };
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponse);
        var httpClient = new HttpClient(mockHttpMessageHandler.Object)
        {
            BaseAddress = new Uri("https://pokeapi.co/api/v2")
        };
        httpClientFactory.Setup(mock => mock.CreateClient(It.IsAny<string>())).Returns(httpClient);

        //assert that description is correct
        var tus = new PokedexService(httpClientFactory.Object);

        var pokemonData = await tus.GetPokemonDataAsync("mewtwo");

        Assert.NotNull(pokemonData);
        Assert.Equal("mewtwo", pokemonData.Name);
        Assert.True(pokemonData.Legendary);
        Assert.Equal("rare", pokemonData.Habitat);
        Assert.Equal(
            "It was created by\\na scientist after\\nyears of horrific\\fgene splicing and\\nDNA engineering\\nexperiments.",
            pokemonData.Description);
    }

    [Fact]
    public async Task GetPokemonDataAsync_ShouldReturnNull_WhenPokemonDataIsNull()
    {
        //setup proxy 
        var httpClientFactory = new Mock<IHttpClientFactory>();
        var httpResponse = new HttpResponseMessage(HttpStatusCode.NotFound);
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponse);
        var httpClient = new HttpClient(mockHttpMessageHandler.Object)
        {
            BaseAddress = new Uri("https://pokeapi.co/api/v2")
        };
        httpClientFactory.Setup(mock => mock.CreateClient(It.IsAny<string>())).Returns(httpClient);

        //assert that description is correct
        var tus = new PokedexService(httpClientFactory.Object);

        var pokemonData = await tus.GetPokemonDataAsync("mewtwooooo");

        Assert.Null(pokemonData);
    }

    // [Theory]
    // public void GetPokemonDataTranslatedAsync_ShouldReturnPokemonData_WhenPokemonDataIsNotNull()
    // {
    //     //setup proxy 
    //     //assert that description is correct
    // }
    //
    // [Theory]
    // public void GetPokemonDataTranslatedAsync_ShouldReturnNull_WhenPokemonDataIsNull()
    // {
    //     //setup proxy 
    //     //assert that description is correct
    // }
}