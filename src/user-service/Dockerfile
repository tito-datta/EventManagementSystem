# Stage 1: Build and Publish the Application
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine as publish

WORKDIR /app

# Copy all files from current directory to /app directory in the image
COPY ./ .

# Perform necessary operations like restoring packages and building the project
RUN dotnet publish ./user-api/user-api.csproj \
        --self-contained true \
        --runtime linux-musl-x64 \
        --configuration Release \
        --output /app/publish

# Stage 2: Create the Runtime Image
FROM mcr.microsoft.com/dotnet/runtime-deps:8.0-alpine

# Install libc6-compat for Alpine to ensure glibc compatibility
RUN apk add libc6-compat icu-libs curl

# Create a non-root user for running the application
RUN adduser --disabled-password \
    --home /app \
    --gecos '' appuser && chown -R appuser /app

# Upgrade musl to ensure compatibility
RUN apk upgrade musl

# Set environment variables for localization
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false \
    LC_ALL=en_US.UTF-8 \
    LANG=en_US.UTF-8

# Set the ASP.NET Core environment variable
ENV ASPNETCORE_URLS=http://+:8080

# Switch to the non-root user
USER appuser

# Set the working directory in the container
WORKDIR /app

# Expose port 8080
EXPOSE 8080

# Copy the published application files from the publish stage
COPY --from=publish /app/publish .

# Define the entry point for the container
ENTRYPOINT ["./user-api"]
