FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS base

RUN apt update && apt -y install clang zlib1g-dev python3-minimal && apt clean
RUN dotnet workload install wasm-tools

WORKDIR /app
RUN chown -R app /app
#USER app
COPY ["tests/Chessy.Benchmarks/*.csproj", "tests/Chessy.Benchmarks/"]
COPY ["src/Chessy.Engine/*.csproj", "src/Chessy.Engine/"]
RUN dotnet restore /app/tests/Chessy.Benchmarks/Chessy.Benchmarks.csproj
COPY . .

WORKDIR "/app/tests/Chessy.Benchmarks"
ENTRYPOINT [ "dotnet", "run", "-c", "release", "Chessy.Benchmarks.csproj" ]
