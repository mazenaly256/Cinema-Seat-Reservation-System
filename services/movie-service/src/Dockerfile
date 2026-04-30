# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /source
COPY movie-service/src/movie-service.csproj movie-service/
RUN dotnet restore movie-service/movie-service.csproj
COPY movie-service/src/ movie-service/
WORKDIR /source/movie-service
RUN dotnet publish -c Release -o /app --no-restore

# Final Stage (Final Image)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "movie-service.dll"]