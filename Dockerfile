#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0-bookworm-slim AS base
RUN mkdir /data
RUN chown 770 /data

WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0-bookworm-slim AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY src/ .
COPY package.json .

FROM build AS host-build
RUN dotnet restore "/src/CleanIAM.Host/CleanIAM.Host.csproj"
RUN apt-get update && apt-get install -y npm
RUN npm install
WORKDIR /src/CleanIAM.Host
RUN dotnet build "CleanIAM.Host.csproj" -c $BUILD_CONFIGURATION -o /app/build /p:PublishReadyToRun=true

FROM host-build AS host-publish
ARG BUILD_CONFIGURATION=Release

WORKDIR /src/CleanIAM.Host
RUN dotnet publish "./CleanIAM.Host.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false /p:PublishReadyToRun=true

FROM base AS host-final
WORKDIR /app
COPY --from=host-publish /app/publish .
ENTRYPOINT ["dotnet", "CleanIAM.Host.dll"]
