FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

USER app
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG configuration=Release

RUN apt update && apt install -y python3-minimal
RUN apt clean && rm -rf /var/lib/apt/lists/*
RUN dotnet workload install wasm-tools

WORKDIR /src
COPY ["Chessy.BlazorWasm/*.csproj", "Chessy.BlazorWasm/"]
COPY ["Chessy.Components/*.csproj", "Chessy.Components/"]
COPY ["Chessy.Engine/*.csproj", "Chessy.Engine/"]

RUN dotnet restore "Chessy.BlazorWasm/Chessy.BlazorWasm.csproj"
COPY . .
WORKDIR "/src/Chessy.BlazorWasm"
RUN dotnet build "Chessy.BlazorWasm.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "Chessy.BlazorWasm.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html
COPY --from=publish /app/publish/wwwroot .

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]

