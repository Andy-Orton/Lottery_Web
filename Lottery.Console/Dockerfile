#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:5.0-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["Lottery.Console/Lottery.ConsoleRunner.csproj", "Lottery.Console/"]
COPY ["Lottery.Actors/Lottery.Actors.csproj", "Lottery.Actors/"]
COPY ["ClassLib/ClassLib.csproj", "ClassLib/"]
RUN dotnet restore "Lottery.Console/Lottery.ConsoleRunner.csproj"
COPY . .
WORKDIR "/src/Lottery.Console"
RUN dotnet build "Lottery.ConsoleRunner.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Lottery.ConsoleRunner.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Lottery.ConsoleRunner.dll"]
