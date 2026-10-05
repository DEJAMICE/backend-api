# Stage 1: Build y restauración de dependencias
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiar el archivo del proyecto y restaurar
COPY ["SafeSignal.Api.csproj", "./"]
RUN dotnet restore "SafeSignal.Api.csproj"

# Copiar todo el código fuente y compilar en modo Release
COPY . .
RUN dotnet build "SafeSignal.Api.csproj" -c Release -o /app/build
RUN dotnet publish "SafeSignal.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: Runtime ligero de producción
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Render asigna el puerto mediante la variable de entorno PORT (por defecto 5000 o 10000)
ENV ASPNETCORE_HTTP_PORTS=5000
EXPOSE 5000

ENTRYPOINT ["dotnet", "SafeSignal.Api.dll"]
