# STAGE 1: BUILD
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /app
COPY . ./
RUN dotnet restore
RUN dotnet publish src/API/API.csproj -c Release -o out /p:UseAppHost=false

# STAGE 2: IMAGEN LIGERA DE EJECUCIÓN
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS final
RUN apk add --no-cache icu-libs
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
WORKDIR /app
COPY --from=build /app/out .
COPY scripts ./scripts
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "API.dll"]
