# build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ./OrderProcessor.Service/*.csproj ./OrderProcessor.Service/
RUN dotnet restore ./OrderProcessor.Service/OrderProcessor.Service.csproj
COPY ./OrderProcessor.Service/. ./OrderProcessor.Service/
WORKDIR /src/OrderProcessor.Service
RUN dotnet publish -c Release -o /app

# run
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "OrderProcessor.Service.dll"]
