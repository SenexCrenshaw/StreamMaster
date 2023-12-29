FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
ARG TARGETPLATFORM
ARG TARGETARCH
ARG BUILDPLATFORM
WORKDIR /app
EXPOSE 7095
ENV ASPNETCORE_URLS=http://+:7095
RUN apt-get update -yq \
    && apt-get upgrade -yq \
    && apt-get install -yq ffmpeg gosu

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG TARGETPLATFORM
ARG TARGETARCH
ARG BUILDPLATFORM
WORKDIR /src
COPY ["StreamMaster.API/StreamMaster.API.csproj", "StreamMaster.API/"]
COPY ["StreamMaster.Application/StreamMaster.Application.csproj", "StreamMaster.Application/"]
COPY ["StreamMaster.Domain/StreamMaster.Domain.csproj", "StreamMaster.Domain/"]
COPY ["StreamMaster.Infrastructure/StreamMaster.Infrastructure.csproj", "StreamMaster.Infrastructure/"]
RUN dotnet restore "StreamMaster.API/StreamMaster.API.csproj" -a $TARGETARCH

COPY . .

WORKDIR "/src/StreamMaster.API"
RUN dotnet build "StreamMaster.API.csproj" -c Debug -o /app/build -a $TARGETARCH
RUN mkdir -p /etc/apt/keyrings
RUN apt-get update -yq \
    && apt-get upgrade -yq \
    && apt-get install -yq ca-certificates curl gnupg git nano

RUN curl -sL https://deb.nodesource.com/gpgkey/nodesource-repo.gpg.key | gpg --dearmor -o /etc/apt/keyrings/nodesource.gpg \
    && echo "deb [signed-by=/etc/apt/keyrings/nodesource.gpg] https://deb.nodesource.com/node_20.x nodistro main" | tee /etc/apt/sources.list.d/nodesource.list \
    && update-ca-certificates \
    && apt-get update && apt-get install -yq nodejs build-essential 

RUN rm -rf /var/lib/apt/lists/* 

WORKDIR /src/streammasterwebui
COPY ["streammasterwebui/", "."]
RUN npm install \
    && npm run build \
    && cp -r dist/* /src/StreamMaster.API/wwwroot/

WORKDIR "/src/StreamMaster.API"
FROM build AS publish
ARG TARGETPLATFORM
ARG TARGETARCH
ARG BUILDPLATFORM
RUN dotnet publish --no-restore "StreamMaster.API.csproj" -c Debug -o /app/publish /p:UseAppHost=false -a $TARGETARCH

FROM base AS final
ARG TARGETPLATFORM
ENV TARGETPLATFORM=${TARGETPLATFORM:-linux/amd64}
ARG TARGETARCH
ARG BUILDPLATFORM
LABEL org.opencontainers.image.url="https://hub.docker.com/r/SenexCrenshaw/streammaster/" \
    org.opencontainers.image.source="https://github.com/SenexCrenshaw/StreamMaster" \
    org.opencontainers.image.vendor="SenexCrenshaw" \
    org.opencontainers.image.title="Stream Master" \
    org.opencontainers.image.description="Dockerized Stream Master by SenexCrenshaw" \
    org.opencontainers.image.authors="SenexCrenshaw"

ENV REACT_API_URL=$REACT_API_URL
ENV STREAMMASTER_BASEHOSTURL=http://localhost:7095/
ENV PUID=0
ENV PGID=0
COPY --from=publish /app/publish .

COPY entrypoint.sh /entrypoint.sh
RUN chmod +x /entrypoint.sh
RUN mkdir /config

ENTRYPOINT ["/entrypoint.sh", "dotnet", "StreamMaster.API.dll"]