# =========================
# Build
# =========================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

# Copy project files first
# This allows Docker to cache the restore layer

COPY ["services/Catalog Service/Catalog/src/Catalog.API/Catalog.API.csproj", "services/Catalog Service/Catalog/src/Catalog.API/"]
COPY ["services/Catalog Service/Catalog/src/Catalog.Application/Catalog.Application.csproj", "services/Catalog Service/Catalog/src/Catalog.Application/"]
COPY ["services/Catalog Service/Catalog/src/Catalog.Domain/Catalog.Domain.csproj", "services/Catalog Service/Catalog/src/Catalog.Domain/"]
COPY ["services/Catalog Service/Catalog/src/Catalog.Infrastructure/Catalog.Infrastructure.csproj", "services/Catalog Service/Catalog/src/Catalog.Infrastructure/"]
COPY ["building-blocks/BuildingBlocks/SharedKernel/SharedKernel.csproj", "building-blocks/BuildingBlocks/SharedKernel/"]



# Restore dependencies
RUN dotnet restore "services/Catalog Service/Catalog/src/Catalog.API/Catalog.API.csproj"

# Copy source code
COPY . .

# Publish API
WORKDIR "/src/services/Catalog Service/Catalog/src/Catalog.API"

RUN dotnet publish "Catalog.API.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore \
    /p:UseAppHost=false


# =========================
# Runtime
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

WORKDIR /app

#===============you removed this code becuse of docker error about linux================
#RUN apt-get update \
#    && apt-get install -y --no-install-recommends libgssapi-krb5-2 \
#    && rm -rf /var/lib/apt/lists/*
#=======================================================================================

COPY --from=build /app/publish .

ENV ASPNETCORE_HTTP_PORTS=8080

EXPOSE 8080

ENTRYPOINT ["dotnet", "Catalog.API.dll"]