import { Epg, Layout } from 'planby';
import React from "react";
import ChannelItem from './ChannelItem';
import ProgramComponent from './ProgramItem';
import Timeline from './Timeline';
import { useApp } from "./useApp";

import { type StreamGroupDto } from '@/lib/iptvApi';
import { useLocalStorage } from "primereact/hooks";
import { StreamGroupSelector } from "../streamGroup/StreamGroupSelector";

const EPGDisplay = (props: EPGDisplayProps) => {
  const [streamGroup, setStreamGroup] = useLocalStorage<StreamGroupDto>({ id: 0 } as StreamGroupDto, 'videoPlayerDialog-streamGroupNumber');

  const { isLoading, getEpgProps, getLayoutProps } = useApp(streamGroup.id);

  const onVideoStreamClick = React.useCallback((videoStreamId: string) => {
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

type EPGDisplayProps = {
  readonly hidden: boolean;
  readonly onChange: ((value: StreamGroupDto) => void);
  readonly onClick: ((videoStreamId: string) => void);
  readonly onMouseEnter?: () => void;
  readonly onMouseLeave?: () => void;
};

export default React.memo(EPGDisplay);
