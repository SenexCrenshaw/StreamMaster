FROM nvidia/cuda:12.3.0-base-ubuntu22.04 as base
ARG TARGETPLATFORM
ARG TARGETARCH
ARG BUILDPLATFORM

FROM senexcrenshaw/streammaster_base:latest AS ffmpeg

WORKDIR /app
EXPOSE 7095

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG TARGETPLATFORM
ARG TARGETARCH
ARG BUILDPLATFORM

WORKDIR /src
COPY ["StreamMasterAPI/StreamMasterAPI.csproj", "StreamMasterAPI/"]
COPY ["StreamMasterApplication/StreamMasterApplication.csproj", "StreamMasterApplication/"]
COPY ["StreamMasterDomain/StreamMasterDomain.csproj", "StreamMasterDomain/"]
COPY ["StreamMasterInfrastructure/StreamMasterInfrastructure.csproj", "StreamMasterInfrastructure/"]
RUN dotnet restore "StreamMasterAPI/StreamMasterAPI.csproj" -a $TARGETARCH

COPY . .

WORKDIR "/src/StreamMasterAPI"
RUN dotnet build "StreamMasterAPI.csproj" -c Debug -o /app/build -a $TARGETARCH

WORKDIR /src/streammasterwebui
COPY ["streammasterwebui/", "."]
RUN mkdir -p /etc/apt/keyrings
RUN apt-get update -yq \
    && apt-get upgrade -yq \
    && apt-get install -yq --no-install-recommends ca-certificates curl gnupg git \
    && curl -sL https://deb.nodesource.com/gpgkey/nodesource-repo.gpg.key | gpg --dearmor -o /etc/apt/keyrings/nodesource.gpg \
    && echo "deb [signed-by=/etc/apt/keyrings/nodesource.gpg] https://deb.nodesource.com/node_20.x nodistro main" | tee /etc/apt/sources.list.d/nodesource.list \
    && apt-get update && apt-get install -yq --no-install-recommends nodejs build-essential \
    && rm -rf /var/lib/apt/lists/* \
    && update-ca-certificates
RUN npm install \
    && npm run build \
    && cp -r dist/* /src/StreamMasterAPI/wwwroot/

WORKDIR "/src/StreamMasterAPI"
FROM build AS publish
ARG TARGETPLATFORM
ARG TARGETARCH
ARG BUILDPLATFORM

RUN dotnet publish --no-restore "StreamMasterAPI.csproj" -c Debug -o /app/publish /p:UseAppHost=false -a $TARGETARCH


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
RUN apt-get update -yq && \
    apt-get install -yq aspnetcore-runtime-7.0 gosu

COPY --from=publish /app/publish .
COPY --from=ffmpeg /usr/bin/ffmpeg /usr/bin/ffmpeg
COPY entrypoint.sh /entrypoint.sh

RUN chmod +x /entrypoint.sh
RUN mkdir /config

ENTRYPOINT ["/entrypoint.sh", "dotnet", "StreamMasterAPI.dll"]
