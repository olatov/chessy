FROM mcr.microsoft.com/dotnet/aspnet:8.0-jammy-chiseled AS base
WORKDIR /app
EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080

USER app
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG configuration=Release
WORKDIR /build
COPY ["src/Chessy.Blazor/*.csproj", "src/Chessy.Blazor/"]
COPY ["src/Chessy.Engine/*.csproj", "src/Chessy.Engine/"]
COPY ["src/Chessy.Components/*.csproj", "src/Chessy.Components/"]
COPY ["src/Chessy.Infrastructure/*.csproj", "src/Chessy.Infrastructure/"]
RUN dotnet restore "src/Chessy.Blazor/Chessy.Blazor.csproj"
COPY . .
WORKDIR "/build/src/Chessy.Blazor"
RUN dotnet build "Chessy.Blazor.csproj" -c "$configuration" -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "Chessy.Blazor.csproj" -c "$configuration" -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Chessy.Blazor.dll"]
