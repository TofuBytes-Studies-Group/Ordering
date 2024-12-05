# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy the solution and project files
COPY *.sln ./
COPY Ordering.API/*.csproj Ordering.API/
COPY Ordering.UnitTests/*.csproj Ordering.UnitTests/
COPY Ordering.Domain/*.csproj Ordering.Domain/
COPY Ordering.Infrastructure/*.csproj Ordering.Infrastructure/
COPY Ordering.IntegrationTests/*.csproj Ordering.IntegrationTests/
COPY Ordering.APITests/*.csproj Ordering.APITests/

# Restore dependencies
RUN dotnet restore

# Copy the rest of the application source code
COPY . ./

# Build and publish the application
RUN dotnet publish Ordering.API/Ordering.API.csproj -c Release -o /app/out

# Stage 2: Create the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copy the build output from the previous stage
COPY --from=build /app/out .

# Expose the port your application listens on
EXPOSE 80

# Set the entry point for the container
ENTRYPOINT ["dotnet", "Ordering.API.dll"]
