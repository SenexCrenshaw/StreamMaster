/* eslint-disable @typescript-eslint/prefer-ts-expect-error */
import React from "react";
import { Epg, Layout } from 'planby';
import ChannelItem from './epg/ChannelItem';
import ProgramComponent from './epg/ProgramItem';
import Timeline from './epg/Timeline';
import { useApp } from "./epg/useApp";
import { StreamGroupSelector } from "./StreamGroupSelector";
import { type StreamGroupDto } from "../store/iptvApi";
import { useLocalStorage } from "primereact/hooks";

const EPGDisplay = (props: EPGDisplayProps) => {
  const [streamGroup, setStreamGroup] = useLocalStorage<StreamGroupDto>({ id: 0 } as StreamGroupDto, 'videoPlayerDialog-streamGroupNumber');

  const { isLoading, getEpgProps, getLayoutProps } = useApp(streamGroup.id);

  const onVideoStreamClick = React.useCallback((videoStreamId: number) => {
    props.onClick?.(videoStreamId);
  }, [props]);

  if (isLoading) {
    return <div className="App">Loading...</div>;
  }

  return (
    <div className="opacity-80" onMouseEnter={props.onMouseEnter} onMouseLeave={props.onMouseLeave} style={{ height: "50vh", width: "100vw" }}>

      {props.hidden !== true && (
        <>
          <StreamGroupSelector
            onChange={(e) => {
              setStreamGroup(e)
            }
            } />
          <Epg isLoading={isLoading} {...getEpgProps()}>
            {/* @ts-ignore */}
            <Layout
              {...getLayoutProps()}
              renderChannel={({ channel }) => (
                <ChannelItem channel={channel} key={channel.uuid} />
              )}
              renderProgram={({ program, ...rest }) => (
                <ProgramComponent key={program.data.id} program={program} {...rest} onClick={onVideoStreamClick} />
              )}
              renderTimeline={(props2) => <Timeline {...props2} />}
            />
          </Epg>
        </>
      )
      }

    </div>
  );
}

EPGDisplay.displayName = 'EPGDisplay';
EPGDisplay.defaultProps = {
};

type EPGDisplayProps = {
  hidden: boolean;
  onChange: ((value: StreamGroupDto) => void);
  onClick: ((videoStreamId: number) => void);
  onMouseEnter?: () => void;
  onMouseLeave?: () => void;
};

export default React.memo(EPGDisplay);
