# Use the official .NET SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy the solution file
COPY Ordering.sln ./

# Copy all project files
COPY Ordering.API/Ordering.API.csproj Ordering.API/
COPY Ordering.UnitTests/Ordering.UnitTests.csproj Ordering.UnitTests/
COPY Ordering.Domain/Ordering.Domain.csproj Ordering.Domain/
COPY Ordering.Infrastructure/Ordering.Infrastructure.csproj Ordering.Infrastructure/
COPY Ordering.IntegrationTests/Ordering.IntegrationTests.csproj Ordering.IntegrationTests/
COPY Ordering.APITests/Ordering.APITests.csproj Ordering.APITests/

# Restore dependencies for all projects
RUN dotnet restore Ordering.sln

# Copy the remaining files and build the application
COPY . ./
RUN dotnet publish Ordering.sln -c Release -o out

# Use the official .NET runtime image to run the application
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build /app/out .

# Set the entry point for the container
ENTRYPOINT ["dotnet", "Ordering.API.dll"]