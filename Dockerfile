# Base runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# copy csproj correctly
COPY ["SimpleUplode/SimpleUplode.csproj", "SimpleUplode/"]
RUN dotnet restore "SimpleUplode/SimpleUplode.csproj"

# copy everything
COPY . .
WORKDIR "/src/SimpleUplode"
RUN dotnet build "SimpleUplode.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish "SimpleUplode.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage
FROM base AS final
WORKDIR /app

# 🔥 IMPORTANT FOR RENDER
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SimpleUplode.dll"]