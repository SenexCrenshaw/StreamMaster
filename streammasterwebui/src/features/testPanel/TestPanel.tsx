/* eslint-disable @typescript-eslint/no-unused-vars */
/* eslint-disable react/no-unused-prop-types */

import { useState, useMemo, memo } from "react";
import { type ColumnMeta } from "../../components/dataSelector/DataSelectorTypes";
import IconSelector from "../../components/selectors/IconSelector";
import { type VideoStreamDto } from "../../store/iptvApi";
import { type PagedResponseOfChannelGroupDto, type ChannelGroupDto, useVideoStreamsGetVideoStreamQuery } from "../../store/iptvApi";
import { useChannelGroupsGetChannelGroupsQuery } from "../../store/iptvApi";
import VideoStreamDataSelector from "../../components/dataSelectors/VideoStreamDataSelector";
import EPGSelector from "../../components/selectors/EPGSelector";
import EPGEditor from "../../components/epg/EPGEditor";
import ChannelLogoEditor from "../../components/ChannelLogoEditor";
import PlayListDataSelector from "../../components/dataSelectors/PlayListDataSelector";
import VideoStreamAddDialog from "../../components/videoStream/VideoStreamAddDialog";
import VideoStreamPanel from "../../components/videoStream/VideoStreamPanel";


const TestPanel = (props: TestPanelProps) => {

  const videoStreamsGetVideoStreamQuery = useVideoStreamsGetVideoStreamQuery('b2aed15ee9db42e715ae3d9e6b815b6a');

  const [dataSource, setDataSource] = useState({} as PagedResponseOfChannelGroupDto);
  const [selectedChannelGroups, setSelectedChannelGroups] = useState<ChannelGroupDto[]>([] as ChannelGroupDto[]);


  const sourceColumns = useMemo((): ColumnMeta[] => {
    return [
      { field: 'name', filter: true, sortable: true },
    ]
  }, []);

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

  return (
    <VideoStreamDataSelector
      channelGroupNames={selectedChannelGroups.map(a => a.name)}
      id="TestPanel"
    />
  );

}

TestPanel.displayName = 'TestPanel';
TestPanel.defaultProps = {
  onChange: null,
  value: null,
};

type TestPanelProps = {
  data?: ChannelGroupDto | undefined;
  onChange?: ((value: string) => void) | null;
  value?: string | null;
};

export default memo(TestPanel);
