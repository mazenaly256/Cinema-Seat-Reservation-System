# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /source
COPY identity-service/src/identity-service.csproj .
RUN dotnet restore identity-service.csproj
COPY identity-service/src/ .
RUN dotnet publish -c Release -o /app --no-restore

# Final Stage (Final Image)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "identity-service.dll"]