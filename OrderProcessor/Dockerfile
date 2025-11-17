# ========= BASE IMAGE (runtime) =========
# If your project targets net8.0, change 9.0 -> 8.0 in both FROM lines.
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

# Optional: document the port your app *expects* to listen on.
# Railway will still route based on the PORT env variable it sets.
EXPOSE 8080

# ========= BUILD IMAGE (SDK) =========
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj files and restore as distinct layers
COPY ["OrderProcessor/OrderProcessor.csproj", "OrderProcessor/"]
COPY ["OrderProcessor.Service/OrderProcessor.Service.csproj", "OrderProcessor.Service/"]

RUN dotnet restore "OrderProcessor/OrderProcessor.csproj"

# Copy the rest of the source
COPY . .

# Build and publish
WORKDIR "/src/OrderProcessor"
RUN dotnet publish "OrderProcessor.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ========= FINAL IMAGE =========
FROM base AS final
WORKDIR /app

COPY --from=build /app/publish .

# Railway will inject PORT; set ASPNETCORE_URLS at runtime to bind to it.
# You can do this via Railway env vars:
#   ASPNETCORE_URLS=http://0.0.0.0:$(PORT)
#
# So we just need the normal entrypoint here:
ENTRYPOINT ["dotnet", "OrderProcessor.dll"]
