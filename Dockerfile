FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["GajinoAgencies.csproj", "."]
RUN dotnet restore "./GajinoAgencies.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "./GajinoAgencies.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./GajinoAgencies.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GajinoAgencies.dll"]


# docker build --build-arg BUILD_CONFIGURATION=Release   -t gajinoagencies_prod .
# docker build --build-arg BUILD_CONFIGURATION=Release   -t gajinoagencies_stg .

#docker run -d --name gajinoagencies_stg  -e ASPNETCORE_ENVIRONMENT=Staging     -p 4150:80 gajinoagencies_stg:latest
#docker run -d --name gajinoagencies_prod -e ASPNETCORE_ENVIRONMENT=Production  -p 6150:80 gajinoagencies_prod:latest
#docker run -d --name gajinoagencies_prod -p 6150:80 gajinoagencies_prod:latest