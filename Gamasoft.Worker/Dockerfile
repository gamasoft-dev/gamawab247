
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime
WORKDIR /app
#EXPOSE 80
#EXPOSE 443

ENV ASPNETCORE_Envoronment=Production
ENV ASPNETCORE_URLS http://+:8001

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Gamasoft.Worker/Gamasoft.Worker.csproj", "Gamasoft.Worker/"]
COPY ["src/Application/Application.csproj", "src/Application/"]
COPY ["src/Infrastructure/Infrastructure.csproj", "src/Infrastructure/"]
COPY ["src/Infrastructure.Data/Infrastructure.Data.csproj", "src/Infrastructure.Data/"]
COPY ["src/Domian/Domain.csproj", "src/Domian/"]
RUN dotnet restore "Gamasoft.Worker/Gamasoft.Worker.csproj"
COPY . .
WORKDIR "/src/Gamasoft.Worker"
RUN dotnet build "Gamasoft.Worker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Gamasoft.Worker.csproj" -c Release -o /app/publish

FROM runtime AS final
WORKDIR /app
COPY --from=publish /app/publish .
#EXPOSE 8001

#ENTRYPOINT ["dotnet", "Gamasoft.Worker.dll"]
CMD ASPNETCORE_URLS=http://*:$PORT dotnet Gamasoft.Worker.dll