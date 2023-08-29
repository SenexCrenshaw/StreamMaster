/* eslint-disable react/no-unused-prop-types */
/* eslint-disable @typescript-eslint/no-unused-vars */

import { useMemo, memo } from "react";
import { GetMessage } from "../../common/common";
import { useVideoStreamsGetVideoStreamsQuery } from "../../store/iptvApi";
import { useChannelNumberColumnConfig, useChannelNameColumnConfig, useChannelLogoColumnConfig } from "../columns/columnConfigHooks";
import DataSelector from "../dataSelector/DataSelector";
import { type ColumnMeta } from "../dataSelector/DataSelectorTypes";

type VideoStreamDataSelectorProps = {
  id: string;
  videoStreamId?: string;
};

const VideoStreamDataSelector = (props: VideoStreamDataSelectorProps) => {
  const dataKey = props.id + '-VideoStreamDataSelector';

  const { columnConfig: channelNumberColumnConfig } = useChannelNumberColumnConfig(false);
  const { columnConfig: channelNameColumnConfig } = useChannelNameColumnConfig(false);
  const { columnConfig: channelLogoColumnConfig } = useChannelLogoColumnConfig(false);


  const targetColumns = useMemo((): ColumnMeta[] => {
    let columnConfigs = [
      channelNumberColumnConfig,
      channelLogoColumnConfig,
      channelNameColumnConfig,
    ];

    return columnConfigs;

  }, [channelLogoColumnConfig, channelNameColumnConfig, channelNumberColumnConfig]);


  const rightHeaderTemplate = useMemo(() => {

    return (
      <div className="flex justify-content-end align-items-center w-full gap-1" />
    );
  }, []);



  return (
    <DataSelector
      columns={targetColumns}
      defaultSortField="user_tvg_name"
      emptyMessage="No Streams"
      headerName={GetMessage('streams')}
      headerRightTemplate={rightHeaderTemplate}
      id={dataKey}
      onSelectionChange={(value, selectAllReturn, retTotalRecords) => {

      }}
      queryFilter={useVideoStreamsGetVideoStreamsQuery}
      selectionMode="multiple"
      showHidden={false}
      style={{ height: 'calc(100vh - 40px)' }}
    />
  );
}

VideoStreamDataSelector.displayName = 'Stream Editor';

export default memo(VideoStreamDataSelector);
