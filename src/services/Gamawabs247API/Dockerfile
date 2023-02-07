#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/services/Gamawabs247API/Gamawabs247API.csproj", "src/services/Gamawabs247API/"]
COPY ["src/Libraries/Application/Application.csproj", "src/Libraries/Application/"]
COPY ["src/Libraries/Infrastructure/Infrastructure.csproj", "src/Libraries/Infrastructure/"]
COPY ["src/Libraries/Infrastructure.Data/Infrastructure.Data.csproj", "src/Libraries/Infrastructure.Data/"]
COPY ["src/Libraries/Domian/Domain.csproj", "src/Libraries/Domian/"]
RUN dotnet restore "src/services/Gamawabs247API/Gamawabs247API.csproj"
COPY . .
WORKDIR "/src/src/services/Gamawabs247API"
RUN dotnet build "Gamawabs247API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Gamawabs247API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Gamawabs247API.dll"]