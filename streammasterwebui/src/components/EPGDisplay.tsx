/* eslint-disable react/no-unused-prop-types */
/* eslint-disable @typescript-eslint/no-unused-vars */
/* eslint-disable @typescript-eslint/consistent-type-imports */
import { Sidebar } from 'primereact/sidebar';
import React from "react";
import * as StreamMasterApi from '../store/iptvApi';
import { Button } from "primereact/button";
import { EPGFilecon, PlayListIcon } from '../common/icons';
import { useEpg, Epg, Layout, Channel, Program } from 'planby';

import { ChannelItem } from './ChannelItem';
import { ProgramComponent } from './ProgramItem';
import { Timeline } from './Timeline';

const EPGDisplay = (props: EPGDisplayProps) => {
  const anchorRef = React.useRef<HTMLDivElement>(null);
  const [isHidden, setIsHidden] = React.useState<boolean>(true);
  const [visibleBottom, setVisibleBottom] = React.useState<boolean>(false);

  const epgForGuide = StreamMasterApi.useStreamGroupsGetStreamGroupEpgForGuideQuery(props.streamGroupNumber);

  React.useEffect(() => {
    if (props.hidden !== isHidden)
      setIsHidden(props.hidden);

    if (props.hidden === false) {
      setVisibleBottom(false);
    }

  }, [isHidden, props.hidden]);

  const channels = React.useMemo((): Channel[] => {
    if (epgForGuide.data) {
      const ret = epgForGuide.data?.channels as Channel[];
      console.log('channels: ', ret);
      return ret;
    }

    return [] as Channel[];
  }, [epgForGuide.data]);


  const programs = React.useMemo((): Program[] => {
    if (epgForGuide.data) {
      const ret = epgForGuide.data?.programs as Program[];
      console.log('programs: ', ret);
      return ret;
    }

    return [] as Program[];
  }, [epgForGuide.data]);


  const StartDate = React.useMemo(() => {
    if (epgForGuide.data) {
      console.log('StartDate: ', epgForGuide.data.startDate)
      console.log(new Date(epgForGuide.data.startDate))
      return new Date(epgForGuide.data.startDate);
    }

    return new Date();

  }, [epgForGuide.data]);


  const EndDate = React.useMemo(() => {
    if (epgForGuide.data) {
      var endDate = StartDate;
      endDate.setHours(endDate.getHours() + 4);
      console.log('endDate: ', endDate)
      return new Date(endDate);
    }

    var today = new Date();
    today.setHours(today.getHours() + 4);
    return today;

  }, [StartDate, epgForGuide.data]);

  const channels2 = React.useMemo(
    () => [
      {
        logo: "http://192.168.1.216:7095/api/files/7/misc-vod-the-bob-ross-channel-vod.png",
        uuid: 'guide2go.114491.schedulesdirect.org'
      },
    ],
    []
  );

  const epg = React.useMemo(
    () => [
      {
        channelUuid: 'guide2go.114491.schedulesdirect.org',
        description:
          'Ut anim nisi consequat minim deserunt...',
        id: 'b67ccaa3-3dd2-4121-8256-33dbddc7f0e6',
        image: 'http://192.168.1.216:7095/api/files/8/https%3a%2f%2fjson.schedulesdirect.org%2f20141201%2fimage%2f880b14b8c3bb785a179e1b8bdcf6a4894793bb326e622dc6d198280683e1a0c3.jpg',
        since: StartDate,
        till: EndDate,
        title: 'Title',

      },
      {
        channelUuid: 'guide2go.114491.schedulesdirect.org',
        description:
          'Ut anim nisi consequat minim deserunt...',
        id: 'b67ccaa3-3dd2-4121-8256-33dbddc7f0e6',
        image: 'http://192.168.1.216:7095/api/files/8/https%3a%2f%2fjson.schedulesdirect.org%2f20141201%2fimage%2f880b14b8c3bb785a179e1b8bdcf6a4894793bb326e622dc6d198280683e1a0c3.jpg',

        since: StartDate.setHours(StartDate.getHours() + 1),
        till: EndDate.setHours(EndDate.getHours() + 24),
        title: 'Title',

      },
    ],
    [EndDate, StartDate]
  );

  const {
    getEpgProps,
    getLayoutProps,
    onScrollToNow,
    onScrollLeft,
    onScrollRight,
  } = useEpg({
    channels: channels,
    dayWidth: 7200,
    endDate: EndDate,
    epg: programs,
    isBaseTimeFormat: true,
    isLine: true,
    isSidebar: true,
    isTimeline: true,
    itemHeight: 80,
    sidebarWidth: 100,
    startDate: StartDate
  });

  if (props.hidden === true) {
    return null;
  }

  return (
    <div className="card flex justify-content-center" ref={anchorRef}  >
      <Button
        hidden={isHidden || visibleBottom === true}
        icon={<PlayListIcon />}
        label="EPG"
        onClick={(e) => {
          console.log(e)
          setVisibleBottom(true);
        }}
        type="button" />

      <Sidebar
        className="w-full h-30rem"
        onHide={() => setVisibleBottom(false)}
        position="bottom"
        visible={visibleBottom}
      >

        <Epg isLoading={epgForGuide.isLoading} {...getEpgProps()}>
          <Layout
            {...getLayoutProps()}
            renderChannel={({ channel }) => (
              <ChannelItem channel={channel} key={channel.uuid} />
            )}
            renderProgram={({ program, ...rest }) => (
              <ProgramComponent key={program.data.id} program={program} {...rest} />
            )}
            renderTimeline={(props2) => <Timeline {...props2} />}
          />
        </Epg>

      </Sidebar>
    </div>
  );
}

EPGDisplay.displayName = 'EPGDisplay';
EPGDisplay.defaultProps = {
  onChange: null,
  value: null,
};

type EPGDisplayProps = {
  hidden: boolean;
  onChange?: ((value: string) => void) | null;
  streamGroupNumber: number;
  value?: string | null;
};

export default React.memo(EPGDisplay);
