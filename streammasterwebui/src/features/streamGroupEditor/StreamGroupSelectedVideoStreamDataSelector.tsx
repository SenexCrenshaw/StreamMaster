import { Tooltip } from "primereact/tooltip";
import { memo, useCallback, useMemo, type CSSProperties } from "react";
import { v4 as uuidv4 } from 'uuid';
import { useSelectedStreamGroup } from "../../app/slices/useSelectedStreamGroup";
import { getColor } from "../../common/colors";
import { GetMessage, getChannelGroupMenuItem } from "../../common/common";
import { GroupIcon } from "../../common/icons";
import { useChannelNameColumnConfig, useChannelNumberColumnConfig, useEPGColumnConfig } from "../../components/columns/columnConfigHooks";
import DataSelector from "../../components/dataSelector/DataSelector";
import { type ColumnMeta } from "../../components/dataSelector/DataSelectorTypes";
import { useStreamGroupVideoStreamsGetStreamGroupVideoStreamsQuery, type VideoStreamDto } from "../../store/iptvApi";
import StreamGroupChannelGroupsSelector from "./StreamGroupChannelGroupsSelector";
import VideoStreamRemoveFromStreamGroupDialog from "./VideoStreamRemoveFromStreamGroupDialog";

type StreamGroupSelectedVideoStreamDataSelectorProps = {
  readonly id: string;
};

const StreamGroupSelectedVideoStreamDataSelector = ({ id }: StreamGroupSelectedVideoStreamDataSelectorProps) => {
  const dataKey = id + '-StreamGroupSelectedVideoStreamDataSelector';
  const { selectedStreamGroup } = useSelectedStreamGroup(id);
  const enableEdit = true;

  const { columnConfig: channelNumberColumnConfig } = useChannelNumberColumnConfig({ enableEdit: enableEdit, useFilter: false });
  const { columnConfig: channelNameColumnConfig } = useChannelNameColumnConfig({ enableEdit: enableEdit, useFilter: false });
  const { columnConfig: epgColumnConfig } = useEPGColumnConfig({ enableEdit: enableEdit, useFilter: false });

  const targetActionBodyTemplate = useCallback((data: VideoStreamDto) => {
    if (data.isReadOnly === true) {
      const tooltipClassName = "grouptooltip-" + uuidv4();
      return (
        <div className='flex min-w-full min-h-full justify-content-end align-items-center'>
          <Tooltip position="left" target={"." + tooltipClassName} >
            {getChannelGroupMenuItem(data.user_Tvg_group, data.user_Tvg_group)}
          </Tooltip>
          <GroupIcon className={tooltipClassName} style={{ color: getColor(data.user_Tvg_group) }} />
        </div >
      );
    }

    return (
      <div className='flex p-0 justify-content-end align-items-center'>
        <VideoStreamRemoveFromStreamGroupDialog id={id} value={data} />
      </div>
    );
  }, [id]);

  const targetColumns = useMemo((): ColumnMeta[] => {

    return [
      channelNumberColumnConfig,
      channelNameColumnConfig,
      epgColumnConfig,
      {
        bodyTemplate: targetActionBodyTemplate, field: 'Remove', header: '', resizeable: false, sortable: false,
        style: {
          maxWidth: '2rem',
        } as CSSProperties,
      }
    ]
  }, [channelNumberColumnConfig, channelNameColumnConfig, epgColumnConfig, targetActionBodyTemplate]);

  const rightHeaderTemplate = () => {
    return (
      <div className="flex justify-content-end align-items-center w-full gap-1" >
        <StreamGroupChannelGroupsSelector streamGroupId={selectedStreamGroup?.id} />
      </div>
    );
  }

  return (
    <DataSelector
      columns={targetColumns}
      defaultSortField="user_tvg_name"
      emptyMessage="No Streams"
      headerName={GetMessage('streams')}
      headerRightTemplate={rightHeaderTemplate()}
      id={dataKey}
      key='rank'
      queryFilter={useStreamGroupVideoStreamsGetStreamGroupVideoStreamsQuery}
      selectedStreamGroupId={selectedStreamGroup?.id ?? 0}
      selectionMode='single'
      style={{ height: 'calc(100vh - 40px)' }
      }
    />
  );
}

StreamGroupSelectedVideoStreamDataSelector.displayName = 'Stream Editor';

export default memo(StreamGroupSelectedVideoStreamDataSelector);


