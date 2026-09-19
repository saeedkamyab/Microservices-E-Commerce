# =========================
# Build
# =========================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

COPY ["BackOffice.Web/BackOffice.Web.csproj", "BackOffice.Web/"]
COPY ["BackOffice.ApiClient/BackOffice.ApiClient.csproj", "BackOffice.ApiClient/"]
COPY ["BackOffice.Components/BackOffice.Components.csproj", "BackOffice.Components/"]
COPY ["BackOffice.Contracts/BackOffice.Contracts.csproj", "BackOffice.Contracts/"]

RUN dotnet restore "BackOffice.Web/BackOffice.Web.csproj"

COPY . .

WORKDIR "/src/BackOffice.Web"

RUN dotnet publish "BackOffice.Web.csproj" \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false


# =========================
# Runtime
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_HTTP_PORTS=8080

EXPOSE 8080

ENTRYPOINT ["dotnet", "BackOffice.Web.dll"]