# build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Moto.Rentals.Api.csproj", "./"]
RUN dotnet restore "./Moto.Rentals.Api.csproj"
COPY . .
RUN dotnet publish "./Moto.Rentals.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Moto.Rentals.Api.dll"]
