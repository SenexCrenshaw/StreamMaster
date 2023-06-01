FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
ARG TARGETPLATFORM
ARG BUILDPLATFORM
WORKDIR /app
EXPOSE 7095
ENV ASPNETCORE_URLS=http://+:7095
RUN apt-get update -yq && apt-get upgrade -yq && apt-get install -yq ffmpeg
# RUN adduser -u 5678 --disabled-password --gecos "" appuser && chown -R appuser /app
# USER appuser

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG TARGETPLATFORM
ARG BUILDPLATFORM
WORKDIR /src
COPY ["StreamMasterAPI/StreamMasterAPI.csproj", "StreamMasterAPI/"]
COPY ["StreamMasterApplication/StreamMasterApplication.csproj", "StreamMasterApplication/"]
COPY ["StreamMasterDomain/StreamMasterDomain.csproj", "StreamMasterDomain/"]
COPY ["StreamMasterInfrastructure/StreamMasterInfrastructure.csproj", "StreamMasterInfrastructure/"]
RUN dotnet restore "StreamMasterAPI/StreamMasterAPI.csproj"
COPY . .
WORKDIR "/src/StreamMasterAPI"
#RUN if [ "$ENV" = "debug" ] ; then dotnet build "StreamMasterAPI.csproj" -c Debug -o /app/build; else dotnet build "StreamMasterAPI.csproj" -c Release -o /app/build; fi
RUN dotnet build "StreamMasterAPI.csproj" -c Debug -o /app/build
# installs NodeJS and NPM
RUN apt-get update -yq && apt-get upgrade -yq && apt-get install -yq curl git nano
RUN curl -sL https://deb.nodesource.com/setup_18.x | bash - && apt-get install -yq nodejs build-essential
WORKDIR /src
COPY ["streammasterwebui/", "streammasterwebui/"]
WORKDIR "/src/streammasterwebui"
RUN npm install
RUN npm run build
RUN cp -r build/* /src/StreamMasterAPI/wwwroot/
WORKDIR "/src/StreamMasterAPI"

FROM --platform=$BUILDPLATFORM build AS publish
# RUN if [ "$ENV" = "debug" ] ; then dotnet publish "StreamMasterAPI.csproj" -c Debug -o /app/publish /p:UseAppHost=false; else dotnet publish "StreamMasterAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false ; fi
ARG TARGETPLATFORM
ARG BUILDPLATFORM
RUN dotnet publish "StreamMasterAPI.csproj" -c Debug -o /app/publish /p:UseAppHost=false;

FROM --platform=$BUILDPLATFORM base AS final
ARG TARGETPLATFORM
ARG BUILDPLATFORM
LABEL       org.opencontainers.image.url="https://hub.docker.com/r/SenexCrenshaw/streammaster/" \
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
