# Base runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj correctly
COPY SimpleUplode/SimpleUplode.csproj SimpleUplode/
RUN dotnet restore SimpleUplode/SimpleUplode.csproj

# Copy rest of the source
COPY . .
WORKDIR /src/SimpleUplode
RUN dotnet publish SimpleUplode.csproj -c Release -o /app/publish /p:UseAppHost=false

# Final stage
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "SimpleUplode.dll"]