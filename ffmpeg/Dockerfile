ARG CUDAVER=12.3.0
ARG UBUNTUVER=22.04

FROM nvidia/cuda:${CUDAVER}-devel-ubuntu${UBUNTUVER} AS build

ENV DEBIAN_FRONTEND noninteractive
ENV NVIDIA_VISIBLE_DEVICES all
ENV NVIDIA_DRIVER_CAPABILITIES compute,utility,video

RUN apt-get update && \
    apt-get upgrade -y && \
    apt-get -y --no-install-recommends install \
    build-essential \
    curl \
    libva-dev \
    python3 \
    python-is-python3 \
    ninja-build \
    meson \
    cmake \
    git 
    # clean 
    # apt-get clean; rm -rf /var/lib/apt/lists/* /tmp/* /var/tmp/* /usr/share/doc/*

# build and move deviceQuery to /usr/bin
RUN mkdir -p /code && \
    git clone --depth 1 https://github.com/NVIDIA/cuda-samples.git /code/cuda-samples && \
    cd /code/cuda-samples/Samples/1_Utilities/deviceQuery && \
    make && \
    mv deviceQuery /usr/local/bin

WORKDIR /app
RUN curl -s  https://raw.githubusercontent.com/markus-perl/ffmpeg-build-script/master/build-ffmpeg -o /app/build-ffmpeg
RUN chmod +x /app/build-ffmpeg

RUN echo $(deviceQuery | grep Capability | head -n 1 | awk 'END {print $NF}' | tr -d '.')

RUN CUDA_COMPUTE_CAPABILITY=$(deviceQuery | grep Capability | head -n 1 | awk 'END {print $NF}' | tr -d '.') SKIPINSTALL=yes /app/build-ffmpeg --build --enable-gpl-and-non-free --full-static

FROM ubuntu:${UBUNTUVER} AS scratch

ENV DEBIAN_FRONTEND noninteractive
ENV NVIDIA_VISIBLE_DEVICES all
ENV NVIDIA_DRIVER_CAPABILITIES compute,utility,video

# # install va-driver
# RUN apt-get update \
#     && apt-get -y install libva-drm2 
    
# RUN apt-get clean; rm -rf /var/lib/apt/lists/* /tmp/* /var/tmp/* /usr/share/doc/*

# Copy ffmpeg
COPY --from=build /app/workspace/bin/ffmpeg /usr/bin/ffmpeg
COPY --from=build /app/workspace/bin/ffprobe /usr/bin/ffprobe
COPY --from=build /app/workspace/bin/ffplay /usr/bin/ffplay

CMD         ["-codecs"]
ENTRYPOINT  ["/usr/bin/ffmpeg"]   
