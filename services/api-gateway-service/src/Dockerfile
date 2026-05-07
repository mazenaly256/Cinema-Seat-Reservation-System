# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /source
COPY api-gateway-service/src/api-gateway-service.csproj .
RUN dotnet restore api-gateway-service.csproj
COPY api-gateway-service/src/ .
RUN dotnet publish -c Release -o /app --no-restore

# Final Stage (Final Image)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "api-gateway-service.dll"]