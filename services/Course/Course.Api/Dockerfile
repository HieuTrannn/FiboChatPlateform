# =============================
# BUILD STAGE
# =============================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy các file .csproj để restore dependencies trước (giúp cache tốt hơn)
COPY ["services/Course/Course.Api/Course.Api.csproj", "services/Course/Course.Api/"]
COPY ["services/Course/Course.Application/Course.Application.csproj", "services/Course/Course.Application/"]
COPY ["services/Course/Course.Domain/Course.Domain.csproj", "services/Course/Course.Domain/"]
COPY ["services/Course/Course.Infrastructure/Course.Infrastructure.csproj", "services/Course/Course.Infrastructure/"]
COPY ["services/Authentication/Domain/Authentication.Domain.csproj", "services/Authentication/Domain/"]
COPY ["cores/Contracts/Contracts.csproj", "cores/Contracts/"]

# Restore dependencies
RUN dotnet restore "services/Course/Course.Api/Course.Api.csproj"

# Copy toàn bộ source code
COPY . .

# Build + publish
WORKDIR "/src/services/Course/Course.Api"
RUN dotnet publish -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false


# =============================
# RUNTIME STAGE
# =============================
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Copy từ stage build
COPY --from=build /app/publish .

# Thiết lập môi trường
ENV ASPNETCORE_URLS=http://+:5002
ENV ASPNETCORE_ENVIRONMENT=Development

# Expose port để container có thể truy cập
EXPOSE 5002

# Entry point khi container khởi động
ENTRYPOINT ["dotnet", "Course.Api.dll"]
