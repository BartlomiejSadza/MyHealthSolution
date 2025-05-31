FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Instalacja curl dla healthcheck
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["MyHealth.Api.csproj", "MyHealth.Api/"]
RUN dotnet restore "MyHealth.Api/MyHealth.Api.csproj"
COPY . MyHealth.Api/
WORKDIR "/src/MyHealth.Api"
RUN dotnet build "MyHealth.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MyHealth.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MyHealth.Api.dll"] 