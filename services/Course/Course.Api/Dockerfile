# services/Course/Course.Api/Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["services/Course/Course.Api/Course.Api.csproj", "services/Course/Course.Api/"]
COPY ["services/Course/Course.Application/Course.Application.csproj", "services/Course/Course.Application/"]
COPY ["services/Course/Course.Domain/Course.Domain.csproj", "services/Course/Course.Domain/"]
COPY ["services/Course/Course.Infrastructure/Course.Infrastructure.csproj", "services/Course/Course.Infrastructure/"]
RUN dotnet restore "./services/Course/Course.Api/Course.Api.csproj"
COPY . .
WORKDIR "/src/services/Course/Course.Api"
RUN dotnet build "./Course.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Course.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Course.Api.dll"]