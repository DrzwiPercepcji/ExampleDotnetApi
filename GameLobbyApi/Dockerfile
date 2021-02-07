#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-alpine AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0.102-ca-patch-buster-slim AS build
WORKDIR /src
COPY ["GameLobbyApi/GameLobbyApi.csproj", "GameLobbyApi/"]
RUN dotnet restore "GameLobbyApi/GameLobbyApi.csproj"
COPY . .
WORKDIR "/src/GameLobbyApi"
RUN dotnet build "GameLobbyApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GameLobbyApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GameLobbyApi.dll"]