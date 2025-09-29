FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
ENV ASPNETCORE_URLS=http://0.0.0.0:${PORT}

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
# Lúc này phải copy theo đường dẫn đầy đủ từ repo root
COPY ["services/Identity/Identity.Api/Identity.Api.csproj", "services/Identity/Identity.Api/"]
RUN dotnet restore "services/Identity/Identity.Api/Identity.Api.csproj"
COPY . .
RUN dotnet publish "services/Identity/Identity.Api/Identity.Api.csproj" -c Release -o /out /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /out .
ENTRYPOINT ["dotnet","Identity.Api.dll"]
