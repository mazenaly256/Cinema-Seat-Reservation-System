# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /source
COPY reservation-service/src/reservation-service.csproj reservation-service/
RUN dotnet restore reservation-service/reservation-service.csproj
COPY reservation-service/src/ reservation-service/
WORKDIR /source/reservation-service
RUN dotnet publish -c Release -o /app --no-restore

# Final Stage (Final Image)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "reservation-service.dll"]