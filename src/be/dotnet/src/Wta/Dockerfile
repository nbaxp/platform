#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 80
ENV ASPNETCORE_URLS=http://*:80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Directory.Packages.props", "."]
COPY ["Nuget.config", "."]
COPY ["src/Wta/Wta.csproj", "src/Wta/"]
COPY ["src/Wta.Application/Wta.Application.csproj", "src/Wta.Application/"]
COPY ["src/Wta.Infrastructure/Wta.Infrastructure.csproj", "src/Wta.Infrastructure/"]
RUN dotnet restore "./src/Wta/./Wta.csproj"
COPY . .
WORKDIR "/src/src/Wta"
RUN dotnet build "./Wta.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Wta.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Wta.dll"]
