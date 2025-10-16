# services/Authentication/API/Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["services/Authentication/API/Authentication.API.csproj", "services/Authentication/API/"]
COPY ["services/Authentication/Application/Authentication.Application.csproj", "services/Authentication/Application/"]
COPY ["services/Authentication/Domain/Authentication.Domain.csproj", "services/Authentication/Domain/"]
COPY ["services/Authentication/Infrastructure/Authentication.Infrastructure.csproj", "services/Authentication/Infrastructure/"]
COPY ["cores/Contracts/Contracts.csproj", "cores/Contracts/"]
COPY ["cores/SharedLibrary/SharedLibrary.csproj", "cores/SharedLibrary/"]
RUN dotnet restore "./services/Authentication/API/Authentication.API.csproj"
COPY . .
WORKDIR "/src/services/Authentication/API"
RUN dotnet build "./Authentication.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Authentication.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Authentication.API.dll"]