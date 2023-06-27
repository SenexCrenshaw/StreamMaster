/* eslint-disable @typescript-eslint/no-unused-vars */
import React from "react";
import * as StreamMasterApi from '../store/iptvApi';
import 'vidstack/styles/defaults.css';
import 'vidstack/styles/community-skin/video.css';

import { MediaCommunitySkin, MediaOutlet, MediaPlayer } from '@vidstack/react';
import EPGDisplay from './EPGDisplay';

import { useLocalStorage } from "primereact/hooks";

const VideoPlayerDialog = (props: VideoPlayerDialogProps) => {
  const [hideEPG, setHideEPG] = React.useState(false);
  const [videoStreamId, setVideoStreamId] = useLocalStorage<number>(-1, 'video-player-videoStreamId');
  const [epgMoused, setEpgMoused] = React.useState<boolean>(true);
  const [src, setSrc] = React.useState<string>("");
  const [poster, setPoster] = React.useState<string>("");
  const [title, setTitle] = React.useState<string>("WOW");
  // eslint-disable-next-line @typescript-eslint/no-unused-vars
  const [streamGroupNumber, setStreamGroupNumber] = React.useState<number>(0);

  const videoStreamsQuery = StreamMasterApi.useVideoStreamsGetVideoStreamsQuery();
  const epgForGuide = StreamMasterApi.useStreamGroupsGetStreamGroupEpgForGuideQuery(streamGroupNumber);

  const getEpg = React.useCallback((channel: string): StreamMasterApi.EpgProgram | undefined => {
    const epg = epgForGuide.data?.programs?.find((p) => p.channelUuid === channel && p.since !== undefined && p.till !== undefined && new Date(p.since) <= new Date() && new Date(p.till) >= new Date());
    return epg;
  }, [epgForGuide.data?.programs]);


  React.useEffect(() => {
    if (videoStreamId !== -1) {
      const videoStream = videoStreamsQuery.data?.find((v) => v.id === videoStreamId);
      if (videoStream) {
        setSrc(videoStream.user_Url);
        const epg = getEpg(videoStream.user_Tvg_ID);
        if (epg !== undefined) {

          if (epg.title !== undefined)
            setTitle(epg.title);

          if (epg.image !== null && epg.image !== undefined)
            setPoster(epg.image);
        }
      }

      props.onChange?.(videoStreamId);
    }
  }, [getEpg, props, videoStreamId, videoStreamsQuery.data]);


  const onVideoStreamClick = React.useCallback((videoStreamIdData: number) => {
    setVideoStreamId(videoStreamIdData);

  }, [setVideoStreamId]);


  const getSource = () => {
    let srcObject = {
      src: src,
      type: 'video/mp4',
    };

    if (src.toLowerCase().endsWith('.ts') || src.toLowerCase().endsWith('.m3u') || src.toLowerCase().endsWith('.m3u8')) {
      srcObject.type = 'application/x-mpegurl';
    }

    return srcObject;
  }

  return (
    <MediaPlayer
      autoplay
      controls
      crossorigin="anonymous"
      onUserIdleChange={(e) => {
        setHideEPG(e.detail)
      }
      }
      poster={poster}
      src={getSource()}
      title={title}
    >
      <MediaOutlet />
      <MediaCommunitySkin />
      <div className="absolute bottom-0 left-0 z-5" style={{ paddingBottom: '5rem', zIndex: '5 !important' }}      >
        <EPGDisplay hidden={hideEPG && !epgMoused}
          onChange={(e) => {
            setStreamGroupNumber(e.id);
            console.log(e);
          }
          }
          onClick={onVideoStreamClick}
          onMouseEnter={() => setEpgMoused(true)}
          onMouseLeave={() => setEpgMoused(false)}
        />
      </div>
    </MediaPlayer >
  );
}

VideoPlayerDialog.displayName = 'VideoPlayerDialog';
VideoPlayerDialog.defaultProps = {
  onChange: null
};

type VideoPlayerDialogProps = {
  onChange?: ((value: number) => void) | null;
};

export default React.memo(VideoPlayerDialog);

