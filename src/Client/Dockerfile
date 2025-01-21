# 1) Build-Stage mit .NET-SDK + Node.js (für TypeScript, falls benötigt)
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
RUN apt-get update && apt-get install -y nodejs

ARG BACKEND_ORIGIN
ENV BACKEND_ORIGIN=$BACKEND_ORIGIN

WORKDIR /src

# Kopiere alle csproj-Dateien, die dein Client braucht
COPY ["src/Application/Application.csproj", "src/Application/"]
COPY ["src/Domain/Domain.csproj", "src/Domain/"]
COPY ["src/Shared/Shared.csproj", "src/Shared/"]
COPY ["src/SDK/SDK.csproj", "src/SDK/"]
COPY ["src/Client/Client.csproj", "src/Client/"]

# Wiederherstellen (Restore) der NuGet-Pakete
RUN dotnet restore "src/Client/Client.csproj"

# Jetzt den ganzen Code kopieren
COPY . .

# Wechsle ins Client-Projektverzeichnis
WORKDIR "/src/src/Client"

# Build/Publish
RUN dotnet publish "Client.csproj" -c Release -o /app/publish /p:BackendOrigin=$BACKEND_ORIGIN

# 2) Final-Stage: NGINX als Webserver
FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html

# Kopiere nur den wwwroot-Ordner der Blazor-App in Nginx
COPY --from=build /app/publish/wwwroot ./

EXPOSE 80
ENTRYPOINT ["nginx", "-g", "daemon off;"]
