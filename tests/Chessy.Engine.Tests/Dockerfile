FROM mcr.microsoft.com/dotnet/sdk:8.0 AS base
WORKDIR /app

USER app
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG configuration=Release
WORKDIR /build
COPY ["tests/Chessy.Engine.Tests/*.csproj", "tests/Chessy.Engine.Tests/"]
COPY ["src/Chessy.Engine/*.csproj", "src/Chessy.Engine/"]
RUN dotnet restore "tests/Chessy.Engine.Tests/Chessy.Engine.Tests.csproj"
COPY . .
WORKDIR "/build/tests/Chessy.Engine.Tests"
RUN dotnet build "Chessy.Engine.Tests.csproj" -c "$configuration" -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "Chessy.Engine.Tests.csproj" -c "$configuration" -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "test", "Chessy.Engine.Tests.dll"]
