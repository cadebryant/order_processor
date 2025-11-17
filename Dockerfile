# ===== Runtime image =====
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

# ===== Build image =====
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy the entire repo into the build context
COPY . .

# Restore and publish the web project
RUN dotnet restore "OrderProcessor/OrderProcessor.csproj"
RUN dotnet publish "OrderProcessor/OrderProcessor.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ===== Final image =====
FROM base AS final
WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "OrderProcessor.dll"]
