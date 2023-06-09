/* eslint-disable @typescript-eslint/no-explicit-any */
/* eslint-disable react/no-unused-prop-types */
/* eslint-disable @typescript-eslint/no-unused-vars */
/* eslint-disable @typescript-eslint/consistent-type-imports */
import React, { useEffect } from "react";
import * as StreamMasterApi from '../store/iptvApi';
import * as Hub from '../store/signlar_functions';
import 'vidstack/styles/defaults.css';
import 'vidstack/styles/community-skin/video.css';
// import VideoJS from './videojs'

import { PlayIcon, PlaylistIcon } from '@vidstack/react/icons';
import { type MediaLoadedMetadataEvent } from 'vidstack';

import { MediaCommunitySkin, MediaOutlet, MediaPlayer, MediaPoster, useMediaProvider, useMediaRemote } from '@vidstack/react';
import EPGDisplay from './EPGDisplay';
import { MediaPlayerElement } from "vidstack";
import { baseHostURL } from "../settings";

const VideoPlayerDialog = (props: VideoPlayerDialogProps) => {
  const [hideEPG, setHideEPG] = React.useState(false);
  const playerRef = React.useRef(null);
  const [videoStreamId, setVideoStreamId] = React.useState(0);
  const [src, setSrc] = React.useState<string>("http://192.168.1.216:7095/api/streamgroups/1/stream/10852.ts");
  const [poster, setPoster] = React.useState<string>("https://media-files.vidstack.io/poster.png");
  const [title, setTitle] = React.useState<string>("WOW");
  const [streamGroupNumber, setStreamGroupNumber] = React.useState<number>(1);

  const videoJsOptions = {
    autoplay: false,
    controls: true,
    fluid: true,

    responsive: true,
    sources: [{
      src: 'http://192.168.1.216:7095/api/streamgroups/1/stream/10852.ts',
      type: "application/x-mpegURL"
    }]
  };


  const handlePlayerReady = (player: any) => {
    playerRef.current = player;


    player.log.level('all')

    // You can handle player events here, for example:
    player.on('waiting', () => {
      console.log('player is waiting');
    });

    player.on('dispose', () => {
      console.log('player will dispose');
    });
  };

  const onVideoStreamClick = React.useCallback((videoStreamIdData: number) => {
    console.log(videoStreamIdData);
    setVideoStreamId(videoStreamIdData);
    const url = `http://192.168.1.216:7095/api/streamgroups/${streamGroupNumber}/stream/${videoStreamIdData}`;
    console.log('setSrc ', url)
    setSrc(url);

    // setPoster(videoStream.poster);
    // setTitle(videoStream.title);


  }, [streamGroupNumber]);


  // return (<VideoJS onReady={handlePlayerReady} options={videoJsOptions} />)

  return (
    <MediaPlayer
      controls
      crossorigin="anonymous"
      onUserIdleChange={(e) => {
        console.log('onUserIdleChange ', e)
        setHideEPG(e.detail)
      }
      }
      poster={poster}
      src={src}
      title={title}

    >
      <MediaOutlet />


      <MediaCommunitySkin />

      <div className="absolute bottom-0 left-0 z-5" style={{ paddingBottom: '5rem', zIndex: '5 !important' }}      >
        <EPGDisplay hidden={hideEPG} onClick={onVideoStreamClick} streamGroupNumber={streamGroupNumber} />
      </div>

    </MediaPlayer >
  );
}

VideoPlayerDialog.displayName = 'VideoPlayerDialog';
VideoPlayerDialog.defaultProps = {
  onChange: null,
  videoUrl: "https://media.w3.org/2010/05/sintel/trailer_hd.mp4",
};

type VideoPlayerDialogProps = {
  data?: StreamMasterApi.ChannelGroupDto | undefined;
  onChange?: ((value: string) => void) | null;
  videoUrl?: string;
};

export default React.memo(VideoPlayerDialog);

