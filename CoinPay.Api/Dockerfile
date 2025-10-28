# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["CoinPay.Api/CoinPay.Api.csproj", "CoinPay.Api/"]
RUN dotnet restore "CoinPay.Api/CoinPay.Api.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/CoinPay.Api"
RUN dotnet build "CoinPay.Api.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "CoinPay.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CoinPay.Api.dll"]
