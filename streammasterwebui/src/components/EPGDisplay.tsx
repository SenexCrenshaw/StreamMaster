/* eslint-disable @typescript-eslint/no-unused-vars */

import React from "react";
import * as StreamMasterApi from '../store/iptvApi';

import { Epg, Layout } from 'planby';
import ChannelItem from './epg/ChannelItem';
import ProgramComponent from './epg/ProgramItem';
import Timeline from './epg/Timeline';
import { useApp } from "./epg/useApp";

const EPGDisplay = (props: EPGDisplayProps) => {

  const { isLoading, getEpgProps, getLayoutProps } = useApp();

  const onVideoStreamClick = React.useCallback((videoStreamId: number) => {
    props.onClick?.(videoStreamId);
  }, [props]);

  if (isLoading) {
    return <div className="App">Loading...</div>;
  }

  return (
    <div className="opacity-80" style={{ height: "50vh", width: "100vw" }}>

      {props.hidden !== true && (
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
      )}

    </div>
  );
}

EPGDisplay.displayName = 'EPGDisplay';
EPGDisplay.defaultProps = {
};

type EPGDisplayProps = {
  hidden: boolean;
  onClick?: ((videoStreamId: number) => void);
  streamGroupNumber: number;
};

export default React.memo(EPGDisplay);
