#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
# WORKDIR /app
# EXPOSE 80
# EXPOSE 443

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


FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Hr.System/Hr.System.csproj", "Hr.System/"]
COPY ["Hr.Infrastructure/Hr.Infrastructure.csproj", "Hr.Infrastructure/"]
COPY ["Hr.Application/Hr.Application.csproj", "Hr.Application/"]
COPY ["Hr.Domain/Hr.Domain.csproj", "Hr.Domain/"]

RUN dotnet restore "./Hr.System/./Hr.System.csproj"
RUN dotnet restore "./Hr.Infrastructure/./Hr.Infrastructure.csproj"
RUN dotnet restore "./Hr.Application/./Hr.Application.csproj"
RUN dotnet restore "./Hr.Domain/./Hr.Domain.csproj"

COPY . .
WORKDIR "/src/Hr.System"
RUN dotnet build "./Hr.System.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Run migrations here
RUN dotnet ef database update --project Hr.Infrastructure --startup-project Hr.System

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Hr.System.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Hr.System.dll"]
