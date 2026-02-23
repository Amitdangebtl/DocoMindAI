# ================= RUNTIME =================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# ================= BUILD =================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# ✅ CORRECT MVC CSPROJ PATH
COPY ["MVCSimpleUplode/MVCSimpleUplode.csproj", "MVCSimpleUplode/"]
RUN dotnet restore "MVCSimpleUplode/MVCSimpleUplode.csproj"

# copy full repo
COPY . .
WORKDIR "/src/MVCSimpleUplode"

RUN dotnet publish "MVCSimpleUplode.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ================= FINAL =================
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "MVCSimpleUplode.dll"]