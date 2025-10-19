# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["services/Course/Course.Api/Course.Api.csproj", "services/Course/Course.Api/"]
COPY ["services/Course/Course.Application/Course.Application.csproj", "services/Course/Course.Application/"]
COPY ["services/Course/Course.Domain/Course.Domain.csproj", "services/Course/Course.Domain/"]
COPY ["services/Authentication/Domain/Authentication.Domain.csproj", "services/Authentication/Domain/"]
COPY ["cores/Contracts/Contracts.csproj", "cores/Contracts/"]
COPY ["services/Course/Course.Infrastructure/Course.Infrastructure.csproj", "services/Course/Course.Infrastructure/"]
RUN dotnet restore "./services/Course/Course.Api/Course.Api.csproj"
COPY . .
WORKDIR "/src/services/Course/Course.Api"
RUN dotnet build "./Course.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Course.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Course.Api.dll"]