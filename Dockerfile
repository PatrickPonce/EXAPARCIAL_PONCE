# Imagen base con SDK para build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copiar csproj y restaurar dependencias
COPY *.csproj .
RUN dotnet restore

# Copiar todo y construir
COPY . .
RUN dotnet publish -c Release -o out

# Imagen base para runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .

# Puerto que escucha la app
ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000

# Comando para iniciar
ENTRYPOINT ["dotnet", "EXAPARCIAL_PONCE.dll"]
