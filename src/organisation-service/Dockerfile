# Stage 1: Build and Publish the Application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files
COPY ["src/access-layer/data-access/", "access-layer/data-access/"]
COPY ["src/models/", "models/"]
COPY ["src/organisation-service/", "organisation-service/"]

# Restore dependencies and publish
RUN dotnet restore "organisation-service/org-api/org-api.csproj" && \
    dotnet publish "organisation-service/org-api/org-api.csproj" -c Release \
    -o /app/publish \
    --self-contained true 

# Stage 2: Create the Runtime Image
FROM mcr.microsoft.com/dotnet/runtime-deps:8.0-jammy-chiseled

WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080

COPY --from=build /app/publish .

# Expose port 8080
EXPOSE 8080

ENTRYPOINT ["./org-api"]