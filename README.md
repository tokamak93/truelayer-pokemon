PokemonProxy
===============

## Setting up

### Clone repository:

```shell
git clone https://github.com/tokamak93/truelayer-pokemon.git
```

### .Net8 sdk:

You can download net8 sdk at https://dotnet.microsoft.com/en-us/download

Follow this guide for your environment: https://learn.microsoft.com/en-us/dotnet/core/install/

## Run

To run the project run:

```shell
dotnet run --project PokemonProxy/PokemonProxy.csproj
```

Then you can try the apis. Example curls:

```
curl --location 'https://localhost:5225/pokemon/mewtwo'
```

```
curl --location 'https://localhost:5225/translated/pokemon/mewtwo'
```

## Test

To test the project run:

```shell
dotnet test PokemonProxy.Test/PokemonProxy.Test.csproj
```

## Docker

Build image with:

```shell
docker build . --tag "pkmn-proxy"
```

Then run with:

```shell
docker run pkmn-proxy:latest -p 8080:8080
```

## Production improvements

Public APIs should have some level of protection to avoid misuse.
In a Production environment there should be an Authorization check, fingerprinting someone is not always safe.
With an identifier token like a JWT you can track and RateLimit requests with a simple API Gateway.
A Caching layer would drastically reduce response time since the pokeApi server is not in the same network as the
ProxyApi and its subject to network latency.
Moreover, Http API are very slow compared to something like gRPC. Unfortunately in this case it is not supported.
This project is very lightweight and has a simple structure. If more API were added the project would need more
structure.
For example:

- separation of API declaration from the handler.
- better separation of modules like a service project or a repository project.
