# =========================
# 🔹 BUILD STAGE
# =========================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar csproj y restaurar dependencias
COPY ["StockTech.API/StockTech.API.csproj", "StockTech.API/"]
RUN dotnet restore "StockTech.API/StockTech.API.csproj"

# Copiar todo el proyecto
COPY . .

# Publicar en modo Release
WORKDIR "/src/StockTech.API"
RUN dotnet publish -c Release -o /app/publish

# =========================
# 🔹 RUNTIME STAGE
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copiar archivos publicados
COPY --from=build /app/publish .

# Puerto que usará Render
ENV ASPNETCORE_URLS=http://+:8080

# Exponer puerto
EXPOSE 8080

# Ejecutar la API
ENTRYPOINT ["dotnet", "StockTech.API.dll"]