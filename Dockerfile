# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project file and restore
COPY src/WhatsAppCrm.Web/WhatsAppCrm.Web.csproj src/WhatsAppCrm.Web/
RUN dotnet restore src/WhatsAppCrm.Web/WhatsAppCrm.Web.csproj

# Copy everything and build
COPY . .
RUN dotnet publish src/WhatsAppCrm.Web/WhatsAppCrm.Web.csproj -c Release -o /app/publish

# Runtime stage — standard Debian-based image (most compatible)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

# Memory optimization for Render free tier (512MB)
ENV ASPNETCORE_ENVIRONMENT=Production
ENV PORT=8080
ENV DOTNET_gcServer=0
ENV DOTNET_GCHeapHardLimit=0x10000000
EXPOSE 8080

ENTRYPOINT ["dotnet", "WhatsAppCrm.Web.dll"]
