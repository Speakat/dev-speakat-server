# docker build -f Speakat.Api/Dockerfile .

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY Speakat.Api/Speakat.Api.csproj                       Speakat.Api/
COPY Speakat.Application/Speakat.Application.csproj       Speakat.Application/
COPY Speakat.Domain/Speakat.Domain.csproj                 Speakat.Domain/
COPY Speakat.Infrastructure/Speakat.Infrastructure.csproj Speakat.Infrastructure/
RUN dotnet restore Speakat.Api/Speakat.Api.csproj

COPY . .
RUN dotnet publish Speakat.Api/Speakat.Api.csproj \
        -c Release \
        -o /app/publish \
        --no-restore \
        /p:UseAppHost=false


FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# 컨테이너 헬스체크용
RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

ENV ASPNETCORE_HTTP_PORTS=8080 \
    DOTNET_gcServer=0 \
    DOTNET_TieredPGO=1

COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "Speakat.Api.dll"]
