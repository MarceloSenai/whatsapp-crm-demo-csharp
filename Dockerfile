# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project file and restore
COPY src/WhatsAppCrm.Web/WhatsAppCrm.Web.csproj src/WhatsAppCrm.Web/
RUN dotnet restore src/WhatsAppCrm.Web/WhatsAppCrm.Web.csproj

# Copy everything and build
COPY . .
RUN dotnet publish src/WhatsAppCrm.Web/WhatsAppCrm.Web.csproj -c Release -o /app/publish

# Runtime stage — alpine for minimal footprint
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS runtime
WORKDIR /app

COPY --from=build /app/publish .

# Render uses PORT env var
ENV ASPNETCORE_ENVIRONMENT=Production
ENV PORT=8080
ENV DOTNET_GCHeapHardLimit=0x10000000
EXPOSE 8080

ENTRYPOINT ["dotnet", "WhatsAppCrm.Web.dll"]
