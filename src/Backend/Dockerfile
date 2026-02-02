# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY src/Backend/FashionEcommerce.Domain/*.csproj ./src/Backend/FashionEcommerce.Domain/
COPY src/Backend/FashionEcommerce.Application/*.csproj ./src/Backend/FashionEcommerce.Application/
COPY src/Backend/FashionEcommerce.Infrastructure/*.csproj ./src/Backend/FashionEcommerce.Infrastructure/
COPY src/Backend/FashionEcommerce.API/*.csproj ./src/Backend/FashionEcommerce.API/
COPY *.sln .

RUN dotnet restore src/Backend/FashionEcommerce.API/FashionEcommerce.API.csproj

# Copy everything else and build
COPY src/Backend/ ./src/Backend/
WORKDIR /app/src/Backend/FashionEcommerce.API
RUN dotnet publish -c Release -o /app/out

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .

# Expose port (Internal Docker Port)
EXPOSE 8080

ENTRYPOINT ["dotnet", "FashionEcommerce.API.dll"]
