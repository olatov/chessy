FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG configuration=Release
WORKDIR /build
COPY ["src/Chessy.Console/*.csproj", "src/Chessy.Console/"]
COPY ["src/Chessy.Engine/*.csproj", "src/Chessy.Engine/"]
RUN dotnet restore "src/Chessy.Console/Chessy.Console.csproj"
COPY . .
WORKDIR "/build/src/Chessy.Console"
RUN dotnet build "Chessy.Console.csproj" -c "$configuration" -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "Chessy.Console.csproj" -c "$configuration" -o /app/publish /p:UseAppHost=false

RUN chown -R app /app
USER app
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Chessy.Console.dll"]
