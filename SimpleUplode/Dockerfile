# ================= BASE (runtime) =================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# ================= BUILD =================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# 👇 FIXED PATH
COPY ["SimpleUplode/SimpleUplode.csproj", "SimpleUplode/"]
RUN dotnet restore "SimpleUplode/SimpleUplode.csproj"

# copy rest of the source
COPY . .
WORKDIR "/src/SimpleUplode"

RUN dotnet build "SimpleUplode.csproj" -c $BUILD_CONFIGURATION -o /app/build

# ================= PUBLISH =================
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "SimpleUplode.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# ================= FINAL =================
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SimpleUplode.dll"]