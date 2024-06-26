FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

USER app
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG configuration=Release

RUN apt update && apt install -y python3-minimal && apt clean && rm -rf /var/lib/apt/lists/*
RUN dotnet workload install wasm-tools

WORKDIR /build
COPY ["src/Chessy.BlazorWasm/*.csproj", "src/Chessy.BlazorWasm/"]
COPY ["src/Chessy.Components/*.csproj", "src/Chessy.Components/"]
COPY ["src/Chessy.Engine/*.csproj", "src/Chessy.Engine/"]
COPY ["src/Chessy.Infrastructure/*.csproj", "src/Chessy.Infrastructure/"]

RUN dotnet restore "src/Chessy.BlazorWasm/Chessy.BlazorWasm.csproj"
COPY . .
WORKDIR "/build/src/Chessy.BlazorWasm"
RUN dotnet build "Chessy.BlazorWasm.csproj" -c "$configuration" -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "Chessy.BlazorWasm.csproj" -c "$configuration" -o /app/publish /p:UseAppHost=false

# nginx
#FROM nginx:alpine AS final
#WORKDIR /usr/share/nginx/html
#COPY --from=publish /app/publish/wwwroot .
#CMD ["nginx", "-g", "daemon off;"]

# lighthttpd
FROM rtsp/lighttpd:latest AS final
WORKDIR /var/www/html/
COPY --from=publish /app/publish/wwwroot .

COPY <<EOF /etc/lighttpd/conf.d/20-deflate.conf
server.modules += ( "mod_deflate" )
deflate.cache-dir = "/tmp/cache/compress/"
deflate.mimetypes = (
    "application/wasm",
    "application/javascript",
    "text/css",
    "text/html",
    "text/plain",
    "application/data"
)
deflate.allowed-encodings = ( "br", "gzip", "deflate" ) # "bzip2" and "zstd" also supported
EOF

COPY <<EOF /etc/lighttpd/conf.d/99-wasm-mime-type.conf
mimetype.assign += (
    ".wasm" => "application/wasm",
    ".dat" => "application/data"
)
EOF
