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

FROM build AS host-build
RUN dotnet restore "/src/Lifeliqe.Host/Lifeliqe.Host.csproj"
WORKDIR /src/Lifeliqe.Host
RUN dotnet build "Lifeliqe.Host.csproj" -c $BUILD_CONFIGURATION -o /app/build /p:PublishReadyToRun=true

FROM host-build AS host-publish
ARG BUILD_CONFIGURATION=Release
COPY --from=host-build src/Lifeliqe.Host/Key_private_default.pem /app/Key_private_default.pem 
COPY --from=host-build src/Lifeliqe.Host/Key_public_default.pem /app/Key_public_default.pem 
COPY --from=host-build src/Lifeliqe.Host/type-sense-import.json /app/type-sense-import.json 

WORKDIR /src/Lifeliqe.Host
RUN dotnet publish "./Lifeliqe.Host.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false /p:PublishReadyToRun=true

FROM base AS host-final
WORKDIR /app
ARG VERSION_TAG="Not Specified"
RUN echo "$VERSION_TAG" > /app/build-version.txt
COPY --from=host-publish /app/Key_private_default.pem /app/Key_private_default.pem
COPY --from=host-publish /app/Key_public_default.pem /app/Key_public_default.pem
COPY --from=host-publish /app/type-sense-import.json /app/type-sense-import.json
COPY --from=host-publish /app/publish .
ENTRYPOINT ["dotnet", "Lifeliqe.Host.dll"]

FROM build AS identity-build
RUN dotnet restore "/src/Lifeliqe.Identity/Lifeliqe.Identity.csproj"
WORKDIR /src/Lifeliqe.Identity
RUN dotnet build "Lifeliqe.Identity.csproj" -c $BUILD_CONFIGURATION -o /app/build /p:PublishReadyToRun=true

FROM identity-build AS identity-publish
ARG BUILD_CONFIGURATION=Release
WORKDIR /src/Lifeliqe.Identity
RUN dotnet publish "./Lifeliqe.Identity.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false /p:PublishReadyToRun=true

FROM base AS identity-final
WORKDIR /app
ARG VERSION_TAG="Not Specified"
RUN echo "$VERSION_TAG" > /app/build-version.txt
COPY --from=identity-publish /app/publish .
ENTRYPOINT ["dotnet", "Lifeliqe.Identity.dll"]
