# syntax=docker/dockerfile:1.7

# ---- build ----
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY LOLTierList.sln ./
COPY Directory.Build.props ./
COPY Directory.Packages.props ./

COPY src/Abstractions/*.csproj     src/Abstractions/
COPY src/Application/*.csproj      src/Application/
COPY src/Domain/*.csproj           src/Domain/
COPY src/Infrastructure/*.csproj   src/Infrastructure/
COPY src/Web/*.csproj              src/Web/

RUN dotnet restore src/Web/Web.csproj -p:RestoreUseStaticGraphEvaluation=true

COPY src/ ./src/

RUN dotnet publish src/Web/Web.csproj \
    -c Release -o /app/publish -p:UseAppHost=false

# ---- runtime ----
FROM --platform=$TARGETPLATFORM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=build /app/publish ./
ENTRYPOINT ["dotnet","LOLTierList.Web.dll"]
