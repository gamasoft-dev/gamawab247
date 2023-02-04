#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/Jobs/Gamasoft.360WebhookConfig/Gamasoft.360WebhookConfig.csproj", "src/Jobs/Gamasoft.360WebhookConfig/"]
COPY ["src/Libraries/Application/Application.csproj", "src/Libraries/Application/"]
COPY ["src/Libraries/Infrastructure/Infrastructure.csproj", "src/Libraries/Infrastructure/"]
COPY ["src/Libraries/Infrastructure.Data/Infrastructure.Data.csproj", "src/Libraries/Infrastructure.Data/"]
COPY ["src/Libraries/Domian/Domain.csproj", "src/Libraries/Domian/"]
RUN dotnet restore "src/Jobs/Gamasoft.360WebhookConfig/Gamasoft.360WebhookConfig.csproj"
COPY . .
WORKDIR "/src/src/Jobs/Gamasoft.360WebhookConfig"
RUN dotnet build "Gamasoft.360WebhookConfig.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Gamasoft.360WebhookConfig.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Gamasoft.360WebhookConfig.dll"]
