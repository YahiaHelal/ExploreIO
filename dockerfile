# Stage 1: Build Angular
FROM node:20-alpine AS angular-build
WORKDIR /app/client

# Copy Angular package files
COPY client/package*.json ./
RUN npm ci --silent

# Copy Angular source
COPY client/ ./

# Build Angular for production (output to dist/client)
RUN npm run build -- --configuration production --output-path=dist/client

# Stage 2: Build .NET
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS dotnet-build
WORKDIR /app/api

# Copy csproj and restore
COPY API/*.csproj ./
RUN dotnet restore

# Copy API source
COPY API/ ./

# Copy Angular built files to wwwroot BEFORE publishing
COPY --from=angular-build /app/client/dist/client ./wwwroot

# Build and publish
RUN dotnet publish -c Release -o /app/publish

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copy published .NET app (includes wwwroot from previous stage)
COPY --from=dotnet-build /app/publish ./

# Expose port
EXPOSE 8080

# Set environment
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Run the app
ENTRYPOINT ["dotnet", "API.dll"]