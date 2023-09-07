/* eslint-disable react/no-unused-prop-types */
/* eslint-disable @typescript-eslint/no-unused-vars */
import { memo, useMemo, useState } from "react";
import { type ColumnMeta } from "../../components/dataSelector/DataSelectorTypes";
import { useVideoStreamsGetVideoStreamQuery, type ChannelGroupDto, type PagedResponseOfChannelGroupDto, type StreamGroupDto } from "../../store/iptvApi";

import StreamGroupChannelGroupsSelector from "../../components/selectors/StreamGroupChannelGroupsSelector";


const TestPanel = (props: TestPanelProps) => {

  const videoStreamsGetVideoStreamQuery = useVideoStreamsGetVideoStreamQuery('b2aed15ee9db42e715ae3d9e6b815b6a');

  const [dataSource, setDataSource] = useState({} as PagedResponseOfChannelGroupDto);
  const [selectedChannelGroups, setSelectedChannelGroups] = useState<ChannelGroupDto[]>([] as ChannelGroupDto[]);

  const selectedStreamGroup = useMemo(((): StreamGroupDto => {
    return { id: 2 } as StreamGroupDto
  }), []);

  const sourceColumns = useMemo((): ColumnMeta[] => {
    return [
      { field: 'name', filter: true, sortable: true },
    ]
  }, []);

  return (
    <StreamGroupChannelGroupsSelector streamGroupId={2} />
  );

  // return (
  //   <VideoStreamPanel />
  // );

  // return (
  //   <StreamGroupSelectedVideoStreamDataSelector
  //     id='testpanel'
  //     streamGroup={selectedStreamGroup}
  //   />
  // );

  // return (
  //   <StreamGroupDataSelectorPicker  />
  // );

  // return (
  //   <VideoStreamAddDialog />
  // );

  // return (
  //   <PlayListDataSelector
  //     id='testpanel'
  //   />
  // );

  // return (
  //   <EPGEditor data={videoStreamsGetVideoStreamQuery.data ?? {} as VideoStreamDto} />
  // );

  // return (
  //   <IconSelector value='https://schedulesdirect-api20141201-logos.s3.dualstack.us-east-1.amazonaws.com/stationLogos/s10240_dark_360w_270h.png' />
  // )

  // return (
  //   <EPGEditor data={videoStreamsGetVideoStreamQuery.data ?? {} as VideoStreamDto} />
  // );

  // return (
  //   <VideoStreamDataSelector
  //     channelGroupNames={selectedChannelGroups.map(a => a.name)}
  //     id="TestPanel"
  //   />
  // );

}

TestPanel.displayName = 'TestPanel';
TestPanel.defaultProps = {
  onChange: null,
  value: null,
};

type TestPanelProps = {
  readonly data?: ChannelGroupDto | undefined;
  readonly onChange?: ((value: string) => void) | null;
  readonly value?: string | null;
};

export default memo(TestPanel);
