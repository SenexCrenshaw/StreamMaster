FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
ARG TARGETPLATFORM
ARG TARGETARCH
ARG BUILDPLATFORM
WORKDIR /app
EXPOSE 7095
ENV ASPNETCORE_URLS=http://+:7095
RUN apt-get update -yq && apt-get upgrade -yq && apt-get install -yq ffmpeg
# RUN adduser -u 5678 --disabled-password --gecos "" appuser && chown -R appuser /app
# USER appuser

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
#RUN if [ "$ENV" = "debug" ] ; then dotnet build "StreamMasterAPI.csproj" -c Debug -o /app/build; else dotnet build "StreamMasterAPI.csproj" -c Release -o /app/build; fi
RUN dotnet build "StreamMasterAPI.csproj" -c Debug -o /app/build -a $TARGETARCH
# installs NodeJS and NPM
RUN apt-get update -yq && apt-get upgrade -yq && apt-get install -yq ca-certificates curl gnupg git nano
RUN curl -fsSL https://deb.nodesource.com/gpgkey/nodesource-repo.gpg.key | sudo gpg --dearmor -o /etc/apt/keyrings/nodesource.gpg
RUN echo "deb [signed-by=/etc/apt/keyrings/nodesource.gpg] https://deb.nodesource.com/node_20.x nodistro main" | sudo tee /etc/apt/sources.list.d/nodesource.list
RUN apt-get update && apt-get install -yq nodejs build-essential
WORKDIR /src
COPY ["streammasterwebui/", "streammasterwebui/"]
WORKDIR "/src/streammasterwebui"
RUN npm install
RUN next build
RUN cp -r build/* /src/StreamMasterAPI/wwwroot/
WORKDIR "/src/StreamMasterAPI"

FROM build AS publish
# RUN if [ "$ENV" = "debug" ] ; then dotnet publish "StreamMasterAPI.csproj" -c Debug -o /app/publish /p:UseAppHost=false; else dotnet publish "StreamMasterAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false ; fi
ARG TARGETPLATFORM
ARG TARGETARCH
ARG BUILDPLATFORM
RUN dotnet publish --no-restore "StreamMasterAPI.csproj" -c Debug -o /app/publish /p:UseAppHost=false -a $TARGETARCH


FROM  base AS final
#FROM base AS final
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
WORKDIR /app
# USER root

# USER appuser
ARG REACT_API_URL
ENV REACT_API_URL=$REACT_API_URL
ENV STREAMMASTER_BASEHOSTURL=http://localhost:7095/
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "StreamMasterAPI.dll"]
