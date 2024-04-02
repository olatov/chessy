FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5083

ENV ASPNETCORE_URLS=http://+:5083

USER app
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["Chessy.Blazor/Chessy.Blazor.csproj", "Chessy.Blazor/"]
COPY ["Chessy.Engine/Chessy.Engine.csproj", "Chessy.Engine/"]
RUN dotnet restore "Chessy.Blazor/Chessy.Blazor.csproj"
COPY . .
WORKDIR "/src/Chessy.Blazor"
RUN dotnet build "Chessy.Blazor.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "Chessy.Blazor.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Chessy.Blazor.dll"]
