FROM --platform=$BUILDPLATFORM docker.io/senexcrenshaw/streammaster:0.9.1-sm AS sm
FROM --platform=$BUILDPLATFORM docker.io/senexcrenshaw/streammaster:0.9.1-base AS base
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

ENV STREAMMASTER_BASEHOSTURL=http://localhost:7095/
ENV PUID=0
ENV PGID=0
COPY --from=sm /app/publish .

COPY docker-entrypoint.sh docker-ensure-initdb.sh /usr/local/bin/
RUN chmod +x /usr/local/bin/docker-entrypoint.sh /usr/local/bin/docker-ensure-initdb.sh
RUN ln -sT docker-ensure-initdb.sh /usr/local/bin/docker-enforce-initdb.sh

COPY entrypoint.sh /entrypoint.sh
COPY env.sh /env.sh
RUN chmod +x /entrypoint.sh /env.sh
RUN mkdir /config

EXPOSE 5432

ENTRYPOINT ["/entrypoint.sh", "dotnet", "StreamMaster.API.dll"]

STOPSIGNAL SIGINT
