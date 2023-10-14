import React from 'react';
import 'vidstack/styles/community-skin/video.css';
import 'vidstack/styles/defaults.css';

import { MediaCommunitySkin, MediaOutlet, MediaPlayer } from '@vidstack/react';
import EPGDisplay from './epg/EPGDisplay';

import {
  EpgProgram,
  VideoStreamsGetPagedVideoStreamsApiArg,
  useStreamGroupsGetStreamGroupEpgForGuideQuery,
  useVideoStreamsGetPagedVideoStreamsQuery,
} from '@lib/iptvApi';
import { useLocalStorage } from 'primereact/hooks';

// import {
//   Player,
//   ControlBar,
//   ReplayControl,
//   ForwardControl,
//   CurrentTimeDisplay,
//   TimeDivider,
//   PlaybackRateMenuButton,
//   VolumeMenuButton
// } from 'video-react';

const VideoPlayerDialog = (props: VideoPlayerDialogProps) => {
  const [hideEPG, setHideEPG] = React.useState(false);
  const [videoStreamId, setVideoStreamId] = useLocalStorage<string>('', 'video-player-videoStreamId');
  const [epgMoused, setEpgMoused] = React.useState<boolean>(true);
  const [src, setSrc] = React.useState<string>('http://shine.jserv.me/live/Senex/CNG4s5XGZZ/50b31b45-30d3-4ee3-b6ea-c7e54b7bc021.ts');
  const [poster, setPoster] = React.useState<string>('');
  const [title, setTitle] = React.useState<string>('WOW');

  const [streamGroupNumber, setStreamGroupNumber] = React.useState<number>(0);

  const videoStreamsQuery = useVideoStreamsGetPagedVideoStreamsQuery({} as VideoStreamsGetPagedVideoStreamsApiArg);
  const epgForGuide = useStreamGroupsGetStreamGroupEpgForGuideQuery(streamGroupNumber);

  const getEpg = React.useCallback(
    (channel: string): EpgProgram | undefined => {
      const epg = epgForGuide.data?.programs?.find(
        (p) => p.channelUuid === channel && p.since !== undefined && p.till !== undefined && new Date(p.since) <= new Date() && new Date(p.till) >= new Date(),
      );

      return epg;
    },
    [epgForGuide.data?.programs],
  );
  console.log('VideoPlayerDialog');

  React.useEffect(() => {
    if (videoStreamId !== '') {
      const videoStream = videoStreamsQuery.data?.data?.find((v) => v.id === videoStreamId);

      if (videoStream) {
        setSrc(videoStream.user_Url);
        const epg = getEpg(videoStream.user_Tvg_ID);

        if (epg !== undefined) {
          if (epg.title !== undefined) setTitle(epg.title);

          if (epg.image !== null && epg.image !== undefined) setPoster(epg.image);
        }
      }

      props.onChange?.(videoStreamId);
    }
  }, [getEpg, props, videoStreamId, videoStreamsQuery.data]);

  const onVideoStreamClick = React.useCallback(
    (videoStreamIdData: string) => {
      setVideoStreamId(videoStreamIdData);
    },
    [setVideoStreamId],
  );

  const getSource = () => {
    let srcObject = {
      src: src,
      type: 'video/mp4',
    };

    if (src.toLowerCase().endsWith('.ts')) {
      srcObject.type = 'video/mpeg';
    }

    if (src.toLowerCase().endsWith('.m3u') || src.toLowerCase().endsWith('.m3u8')) {
      srcObject.type = 'application/x-mpegurl';
    }

    return srcObject;
  };

  // return (
  //   <ReactPlayer
  //     controls
  //     height="100%"
  //     url={src}
  //     width="100%"
  //   />
  // )

  return (
    <MediaPlayer
      autoplay
      controls
      crossorigin="anonymous"
      onUserIdleChange={(e) => {
        setHideEPG(e.detail);
      }}
      poster={poster}
      src={getSource()}
      title={title}
    >
      <MediaOutlet />
      <MediaCommunitySkin />
      <div className="absolute bottom-0 left-0 z-5" style={{ paddingBottom: '5rem', zIndex: '5 !important' }}>
        <EPGDisplay
          hidden={hideEPG && !epgMoused}
          onChange={(e) => {
            setStreamGroupNumber(e.id);
            console.log(e);
          }}
          onClick={onVideoStreamClick}
          onMouseEnter={() => setEpgMoused(true)}
          onMouseLeave={() => setEpgMoused(false)}
        />
      </div>
    </MediaPlayer>
  );
};

VideoPlayerDialog.displayName = 'VideoPlayerDialog';

type VideoPlayerDialogProps = {
  readonly onChange?: ((value: string) => void) | null;
};

export default React.memo(VideoPlayerDialog);
