FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
RUN apt-get update -y
RUN apt-get install clang zlib1g-dev -y # needed for AOT dependencies
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["PokemonProxy/PokemonProxy.csproj", "PokemonProxy/"]
RUN dotnet restore "PokemonProxy/PokemonProxy.csproj"
COPY . .
WORKDIR "/src/PokemonProxy"
RUN dotnet build "PokemonProxy.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "PokemonProxy.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=true

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish/ .
ENTRYPOINT ["./PokemonProxy"]
