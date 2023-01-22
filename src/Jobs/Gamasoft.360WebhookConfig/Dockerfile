#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Gamasoft.360WebhookConfig/Gamasoft.360WebhookConfig.csproj", "Gamasoft.360WebhookConfig/"]
COPY ["src/Application/Application.csproj", "src/Application/"]
COPY ["src/Infrastructure/Infrastructure.csproj", "src/Infrastructure/"]
COPY ["src/Infrastructure.Data/Infrastructure.Data.csproj", "src/Infrastructure.Data/"]
COPY ["src/Domian/Domain.csproj", "src/Domian/"]
RUN dotnet restore "Gamasoft.360WebhookConfig/Gamasoft.360WebhookConfig.csproj"
COPY . .
WORKDIR "/src/Gamasoft.360WebhookConfig"
RUN dotnet build "Gamasoft.360WebhookConfig.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Gamasoft.360WebhookConfig.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
#ENTRYPOINT ["dotnet", "Gamasoft.360WebhookConfig.dll"]
CMD ASPNETCORE_URLS=http://*:$PORT dotnet Gamasoft.360WebhookConfig.dll