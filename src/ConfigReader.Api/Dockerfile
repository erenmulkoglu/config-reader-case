FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ConfigReaderCase.sln .
COPY src/ConfigReader/ConfigReader.csproj src/ConfigReader/
COPY src/ConfigReader.Api/ConfigReader.Api.csproj src/ConfigReader.Api/

RUN dotnet restore src/ConfigReader.Api/ConfigReader.Api.csproj

COPY src/ConfigReader src/ConfigReader
COPY src/ConfigReader.Api src/ConfigReader.Api

RUN dotnet publish src/ConfigReader.Api/ConfigReader.Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "ConfigReader.Api.dll"]