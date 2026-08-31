FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY Nexus.Domain/Nexus.Domain.csproj Nexus.Domain/
COPY Nexus.Application/Nexus.Application.csproj Nexus.Application/
COPY Nexus.Infrastructure/Nexus.Infrastructure.csproj Nexus.Infrastructure/
COPY Nexus.API/Nexus.API.csproj Nexus.API/
RUN dotnet restore Nexus.API/Nexus.API.csproj

COPY . .
RUN dotnet publish Nexus.API/Nexus.API.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Nexus.API.dll"]
