# build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Mottu.Rentals.Api.csproj", "./"]
RUN dotnet restore "./Mottu.Rentals.Api.csproj"
COPY . .
RUN dotnet publish "./Mottu.Rentals.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Mottu.Rentals.Api.dll"]
