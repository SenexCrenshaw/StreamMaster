/* eslint-disable @typescript-eslint/no-explicit-any */
/* eslint-disable react/no-unused-prop-types */
/* eslint-disable @typescript-eslint/no-unused-vars */
/* eslint-disable @typescript-eslint/consistent-type-imports */
// import 'video-react/dist/video-react.css';
import React, { useEffect } from "react";
import * as StreamMasterApi from '../store/iptvApi';
import * as Hub from '../store/signlar_functions';
import 'vidstack/styles/defaults.css';
import 'vidstack/styles/community-skin/video.css';
import { PlaylistIcon } from '@vidstack/react/icons';
import { type MediaLoadedMetadataEvent } from 'vidstack';

import { MediaCommunitySkin, MediaOutlet, MediaPlayer, MediaPoster, useMediaProvider, useMediaRemote } from '@vidstack/react';
import EPGDisplay from './EPGDisplay';
import { MediaPlayerElement } from "vidstack";

const VideoPlayerDialog = (props: VideoPlayerDialogProps) => {
  const [hideEPG, setHideEPG] = React.useState(false);
  // useEffect(() => {
  //   const handleMouseMove = (event: MouseEvent) => {
  //     const screenWidth = window.innerWidth;
  //     const mouseX = event.pageX;

  //     if (mouseX <= screenWidth * 0.1) {
  //       setShowEPG(true);
  //       // Mouse is on the left 10% of the screen
  //       // Trigger your action here
  //     } else {
  //       setShowEPG(false);
  //     }
  //   };

  //   document.addEventListener('mousemove', handleMouseMove);

  //   return () => {
  //     // Cleanup the event listener when the component unmounts
  //     document.removeEventListener('mousemove', handleMouseMove);
  //   };
  // }, []);

  return (

    <MediaPlayer
      aspect-ratio={16 / 9}
      className='col-12'
      crossorigin=""
      onUserIdleChange={(e) => {
        console.log('onUserIdleChange ', e)
        setHideEPG(e.detail)
      }
      }
      poster="https://image.mux.com/VZtzUzGRv02OhRnZCxcNg49OilvolTqdnFLEqBsTwaxU/thumbnail.webp?time=268&width=980"
      src="https://stream.mux.com/VZtzUzGRv02OhRnZCxcNg49OilvolTqdnFLEqBsTwaxU/low.mp4"
      thumbnails="https://media-files.vidstack.io/sprite-fight/thumbnails.vtt"
      title="Sprite Fight"
    >
      <MediaOutlet>
        <MediaPoster
          alt="Girl walks into sprite gnomes around her friend on a campfire in danger!"
        />
        <track
          default
          kind="subtitles"
          label="English"
          src="https://media-files.vidstack.io/sprite-fight/subs/english.vtt"
          srcLang="en-US"
        />
        <track
          default
          kind="chapters"
          src="https://media-files.vidstack.io/sprite-fight/chapters.vtt"
          srcLang="en-US"
        />
      </MediaOutlet>

      <MediaCommunitySkin />
      <div className="absolute bottom-0 left-0 z-5"
        style={{ paddingBottom: '5rem', zIndex: '5 !important' }}
      >
        <EPGDisplay hidden={hideEPG} streamGroupNumber={1} />
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
