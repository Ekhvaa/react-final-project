FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY TourApi.sln ./
COPY src/TourApi.Api/TourApi.Api.csproj src/TourApi.Api/
COPY src/TourApi.Core/TourApi.Core.csproj src/TourApi.Core/
COPY src/TourApi.Infrastructure/TourApi.Infrastructure.csproj src/TourApi.Infrastructure/
COPY src/TourApi.Common/TourApi.Common.csproj src/TourApi.Common/
COPY src/TourApi.Migrations/TourApi.Migrations.csproj src/TourApi.Migrations/

RUN dotnet restore TourApi.sln

COPY . .

RUN dotnet publish src/TourApi.Api/TourApi.Api.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "TourApi.Api.dll"]