/* eslint-disable @typescript-eslint/no-unused-vars */
/* eslint-disable react/no-unused-prop-types */

import { useState, useMemo, memo } from "react";
import { type ColumnMeta } from "../../components/dataSelector/DataSelectorTypes";
import IconSelector from "../../components/selectors/IconSelector";
import { type PagedResponseOfChannelGroupDto, type ChannelGroupDto } from "../../store/iptvApi";
import { useChannelGroupsGetChannelGroupsQuery } from "../../store/iptvApi";
import VideoStreamDataSelector from "../../components/videoStream/VideoStreamDataSelector";

const TestPanel = (props: TestPanelProps) => {

  const [dataSource, setDataSource] = useState({} as PagedResponseOfChannelGroupDto);
  const [selectedChannelGroups, setSelectedChannelGroups] = useState<ChannelGroupDto[]>([] as ChannelGroupDto[]);


  const sourceColumns = useMemo((): ColumnMeta[] => {
    return [
      { field: 'name', filter: true, sortable: true },
    ]
  }, []);

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
