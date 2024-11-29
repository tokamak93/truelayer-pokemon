using System.Net;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Moq.Protected;
using PokemonProxy.Services.Implementations;
using PokemonProxy.Services.Model;

namespace PokemonProxy.Test;

public class PokedexServiceTest
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
        }
    }

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
                            "It was created by\na scientist after\nyears of horrific\fgene splicing and\nDNA engineering\nexperiments.",
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
            "It was created by a scientist after years of horrific gene splicing and DNA engineering experiments.",
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

    [Fact]
    public async Task GetPokemonDataTranslatedAsync_ShouldReturnPokemonData_WhenPokemonDataIsNotNull()
    {
        //setup proxy 
        var httpClientFactory = new Mock<IHttpClientFactory>();
        var httpResponsePokeApi = new HttpResponseMessage(HttpStatusCode.Accepted)
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
                            "It was created by\na scientist after\nyears of horrific\fgene splicing and\nDNA engineering\nexperiments.",
                        Language = new Language { Name = "en" }
                    }
                ]
            }))
        };

        var httpResponseFunTranslationApi = new HttpResponseMessage(HttpStatusCode.Accepted)
        {
            Content = new StringContent(JsonSerializer.Serialize(new TranslatedPokemonDescriptionResponseBody
            {
                Contents = new Contents
                {
                    Translated =
                        "Created by a scientist after years of horrific gene splicing and dna engineering experiments,  it was."
                }
            }))
        };
        var mockHttpMessageHandlerPokeApi = new Mock<HttpMessageHandler>();
        mockHttpMessageHandlerPokeApi.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponsePokeApi);
        var httpClientPokeApi = new HttpClient(mockHttpMessageHandlerPokeApi.Object)
        {
            BaseAddress = new Uri("https://pokeapi.co/api/v2")
        };
        var mockHttpMessageHandlerFunTranslateApi = new Mock<HttpMessageHandler>();
        mockHttpMessageHandlerFunTranslateApi.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponseFunTranslationApi);
        var httpClientFunTranslationApi = new HttpClient(mockHttpMessageHandlerFunTranslateApi.Object)
        {
            BaseAddress = new Uri("https://api.funtranslations.com/translate")
        };
        httpClientFactory.Setup(mock => mock.CreateClient(It.Is<string>(x => x == "PokeApi")))
            .Returns(httpClientPokeApi);
        httpClientFactory.Setup(mock => mock.CreateClient(It.Is<string>(x => x == "FunTranslationApi")))
            .Returns(httpClientFunTranslationApi);

        //assert that description is correct
        var tus = new PokedexService(httpClientFactory.Object);

        var pokemonData = await tus.GetPokemonDataTranslatedAsync("mewtwo");

        Assert.NotNull(pokemonData);
        Assert.Equal("mewtwo", pokemonData.Name);
        Assert.True(pokemonData.Legendary);
        Assert.Equal("rare", pokemonData.Habitat);
        Assert.Equal(
            "Created by a scientist after years of horrific gene splicing and dna engineering experiments,  it was.",
            pokemonData.Description);
    }

    [Fact]
    public async Task GetPokemonDataTranslatedAsync_ShouldReturnNull_WhenPokemonDataIsNull()
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

        var pokemonData = await tus.GetPokemonDataTranslatedAsync("mewtwooooo");

        Assert.Null(pokemonData);
    }

    [Fact]
    public async Task GetPokemonDataTranslatedAsync_ShouldReturnPokemonDataUnchanges_WhenApiNotAvailable()
    {
        //setup proxy 
        var httpClientFactory = new Mock<IHttpClientFactory>();
        var httpResponsePokeApi = new HttpResponseMessage(HttpStatusCode.Accepted)
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
                            "It was created by\na scientist after\nyears of horrific\fgene splicing and\nDNA engineering\nexperiments.",
                        Language = new Language { Name = "en" }
                    }
                ]
            }))
        };

        var httpResponseFunTranslationApi = new HttpResponseMessage(HttpStatusCode.BadGateway);
        var mockHttpMessageHandlerPokeApi = new Mock<HttpMessageHandler>();
        mockHttpMessageHandlerPokeApi.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponsePokeApi);
        var httpClientPokeApi = new HttpClient(mockHttpMessageHandlerPokeApi.Object)
        {
            BaseAddress = new Uri("https://pokeapi.co/api/v2")
        };
        var mockHttpMessageHandlerFunTranslateApi = new Mock<HttpMessageHandler>();
        mockHttpMessageHandlerFunTranslateApi.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponseFunTranslationApi);
        var httpClientFunTranslationApi = new HttpClient(mockHttpMessageHandlerFunTranslateApi.Object)
        {
            BaseAddress = new Uri("https://api.funtranslations.com/translate")
        };
        httpClientFactory.Setup(mock => mock.CreateClient("PokeApi")).Returns(httpClientPokeApi);
        httpClientFactory.Setup(mock => mock.CreateClient("FunTranslationApi")).Returns(httpClientFunTranslationApi);

        //assert that description is correct
        var tus = new PokedexService(httpClientFactory.Object);

        var pokemonData = await tus.GetPokemonDataTranslatedAsync("mewtwo");

        Assert.NotNull(pokemonData);
        Assert.Equal("mewtwo", pokemonData.Name);
        Assert.True(pokemonData.Legendary);
        Assert.Equal("rare", pokemonData.Habitat);
        Assert.Equal(
            "It was created by a scientist after years of horrific gene splicing and DNA engineering experiments.",
            pokemonData.Description);
    }
}