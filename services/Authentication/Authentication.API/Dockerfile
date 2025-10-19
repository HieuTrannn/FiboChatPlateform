# =============================
# BUILD STAGE
# =============================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["services/Authentication/Authentication.API/Authentication.API.csproj", "services/Authentication/Authentication.API/"]
COPY ["services/Authentication/Authentication.Application/Authentication.Application.csproj", "services/Authentication/Authentication.Application/"]
COPY ["services/Authentication/Authentication.Domain/Authentication.Domain.csproj", "services/Authentication/Authentication.Domain/"]
COPY ["services/Authentication/Authentication.Infrastructure/Authentication.Infrastructure.csproj", "services/Authentication/Authentication.Infrastructure/"]
COPY ["cores/Contracts/Contracts.csproj", "cores/Contracts/"]
COPY ["cores/SharedLibrary/SharedLibrary.csproj", "cores/SharedLibrary/"]


RUN dotnet restore "services/Authentication/Authentication.API/Authentication.API.csproj"

# Copy all source code
COPY . .

# Build + Publish
WORKDIR "/src/services/Authentication/API"
RUN dotnet publish -c Release -o /app/out

# =============================
# RUNTIME STAGE
# =============================
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .

ENV ASPNETCORE_URLS=http://+:5001
EXPOSE 5001

ENTRYPOINT ["dotnet", "Authentication.API.dll"]
