
# FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
# WORKDIR /app
# EXPOSE 8080
# EXPOSE 8081

# FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
# ARG BUILD_CONFIGURATION=Release
# WORKDIR /src
# COPY ["Hr.System/Hr.System.csproj", "Hr.System/"]
# COPY ["Hr.Infrastructure/Hr.Infrastructure.csproj", "Hr.Infrastructure/"]
# COPY ["Hr.Application/Hr.Application.csproj", "Hr.Application/"]
# COPY ["Hr.Domain/Hr.Domain.csproj", "Hr.Domain/"]

# RUN dotnet restore "./Hr.System/./Hr.System.csproj"
# RUN dotnet restore "./Hr.Infrastructure/./Hr.Infrastructure.csproj"
# RUN dotnet restore "./Hr.Application/./Hr.Application.csproj"
# RUN dotnet restore "./Hr.Domain/./Hr.Domain.csproj"

# COPY . .
# WORKDIR "/src/Hr.System"
# RUN dotnet build "./Hr.System.csproj" -c $BUILD_CONFIGURATION -o /app/build

# FROM build AS publish
# ARG BUILD_CONFIGURATION=Release
# RUN dotnet publish "./Hr.System.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# FROM base AS final
# WORKDIR /app
# COPY --from=publish /app/publish .
# ENTRYPOINT ["dotnet", "Hr.System.dll"]

#---------------------------------------------------
# Define base image for build stage
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy only project files to leverage Docker cache
COPY ["Hr.System/Hr.System.csproj", "Hr.System/"]
COPY ["Hr.Infrastructure/Hr.Infrastructure.csproj", "Hr.Infrastructure/"]
COPY ["Hr.Application/Hr.Application.csproj", "Hr.Application/"]
COPY ["Hr.Domain/Hr.Domain.csproj", "Hr.Domain/"]

# Restore dependencies
RUN dotnet restore "./Hr.System/Hr.System.csproj"

# Copy the entire source code
COPY . .

# Build the application
RUN dotnet build "./Hr.System/Hr.System.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Define base image for runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS final
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Install curl for health check and clean up
RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

# Copy build artifacts from build stage
COPY --from=build /app/build .

# Health check
HEALTHCHECK --interval=5s --timeout=10s --retries=3 CMD curl --silent --fail http://localhost/health || exit 1

# Copy and set permissions for wait-for.sh
# COPY wait-for.sh .
# RUN chmod +x wait-for.sh

# Define entry point
# ENTRYPOINT ["./wait-for.sh", "localhost:1433", "--", "dotnet", "Hr.System.dll"]
ENTRYPOINT [ "dotnet", "Hr.System.dll"]
