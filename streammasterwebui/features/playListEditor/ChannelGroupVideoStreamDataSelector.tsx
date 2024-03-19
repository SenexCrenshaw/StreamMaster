import { useChannelGroupColumnConfig } from '@components/columns/useChannelGroupColumnConfig';
import { useChannelLogoColumnConfig } from '@components/columns/useChannelLogoColumnConfig';
import { useChannelNameColumnConfig } from '@components/columns/useChannelNameColumnConfig';
import { useChannelNumberColumnConfig } from '@components/columns/useChannelNumberColumnConfig';
import { useEPGColumnConfig } from '@components/columns/useEPGColumnConfig';
import { useM3UFileNameColumnConfig } from '@components/columns/useM3UFileNameColumnConfig';
import DataSelector from '@components/dataSelector/DataSelector';
import { ColumnMeta } from '@components/dataSelector/DataSelectorTypes';
import { TriSelectShowHidden } from '@components/selectors/TriSelectShowHidden';
import AutoSetChannelNumbers from '@components/videoStream/AutoSetChannelNumbers';
import VideoStreamAddDialog from '@components/videoStream/VideoStreamAddDialog';
import VideoStreamCopyLinkDialog from '@components/videoStream/VideoStreamCopyLinkDialog';
import VideoStreamDeleteDialog from '@components/videoStream/VideoStreamDeleteDialog';
import VideoStreamEditDialog from '@components/videoStream/VideoStreamEditDialog';
import VideoStreamResetLogoDialog from '@components/videoStream/VideoStreamResetLogoDialog';
import VideoStreamResetLogosDialog from '@components/videoStream/VideoStreamResetLogosDialog';
import VideoStreamSetAutoSetEPGDialog from '@components/videoStream/VideoStreamSetAutoSetEPGDialog';
import VideoStreamSetLogoFromEPGDialog from '@components/videoStream/VideoStreamSetLogoFromEPGDialog';
import VideoStreamSetLogosFromEPGDialog from '@components/videoStream/VideoStreamSetLogosFromEPGDialog';
import VideoStreamSetTimeShiftDialog from '@components/videoStream/VideoStreamSetTimeShiftDialog';
import VideoStreamSetTimeShiftsDialog from '@components/videoStream/VideoStreamSetTimeShiftsDialog';
import VideoStreamVisibleDialog from '@components/videoStream/VideoStreamVisibleDialog';
import { GetMessage, arraysContainSameStrings } from '@lib/common/common';
import { ChannelGroupDto, VideoStreamDto, useVideoStreamsGetPagedVideoStreamsQuery } from '@lib/iptvApi';
import { useQueryAdditionalFilters } from '@lib/redux/slices/useQueryAdditionalFilters';
import { useSelectedItems } from '@lib/redux/slices/useSelectedItemsSlice';
import { useSelectedVideoStreams } from '@lib/redux/slices/useSelectedVideoStreams';
import { memo, useCallback, useEffect, useMemo, useState } from 'react';
import PlayListDataSelectorDropDown from './PlayListDataSelectorDropDown';

import '@lib/styles/theme.css'; // theme

interface ChannelGroupVideoStreamDataSelectorProperties {
  readonly enableEdit?: boolean;
  readonly id: string;
  readonly reorderable?: boolean;
}

const ChannelGroupVideoStreamDataSelector = ({ enableEdit: propsEnableEdit, id, reorderable }: ChannelGroupVideoStreamDataSelectorProperties) => {
  const dataKey = `${id}-ChannelGroupVideoStreamDataSelector`;

  const { selectSelectedItems } = useSelectedItems<ChannelGroupDto>('selectSelectedChannelGroupDtoItems');
  const [enableEdit, setEnableEdit] = useState<boolean>(true);

  const { columnConfig: channelNameColumnConfig } = useChannelNameColumnConfig({ enableEdit });
  const { columnConfig: m3uFileNameColumnConfig } = useM3UFileNameColumnConfig({ enableEdit: false });
  const { columnConfig: epgColumnConfig } = useEPGColumnConfig({ enableEdit });
  const { columnConfig: channelNumberColumnConfig } = useChannelNumberColumnConfig({ enableEdit, useFilter: false });
  const { columnConfig: channelLogoColumnConfig } = useChannelLogoColumnConfig({ enableEdit });

  const channelGroupNames = useMemo(() => selectSelectedItems.map((channelGroup) => channelGroup.name), [selectSelectedItems]);
  const { columnConfig: channelGroupConfig } = useChannelGroupColumnConfig({ enableEdit });
  const { queryAdditionalFilter, setQueryAdditionalFilter } = useQueryAdditionalFilters(dataKey);
  const { setSelectedVideoStreams } = useSelectedVideoStreams(dataKey);

  useEffect(() => {
    if (!arraysContainSameStrings(queryAdditionalFilter?.values, channelGroupNames)) {
      setQueryAdditionalFilter({
        field: 'user_Tvg_group',
        matchMode: 'equals',
        values: channelGroupNames
      });
    }
  }, [channelGroupNames, dataKey, queryAdditionalFilter, setQueryAdditionalFilter]);

  useEffect(() => {
    if (propsEnableEdit !== enableEdit) {
      setEnableEdit(propsEnableEdit ?? true);
    }

    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [propsEnableEdit]);

  const targetActionBodyTemplate = useCallback(
    (data: VideoStreamDto) => (
      <div className="flex p-0 justify-content-end align-items-center">
        <VideoStreamCopyLinkDialog value={data} />
        <VideoStreamSetTimeShiftDialog iconFilled={false} value={data} />
        <VideoStreamResetLogoDialog value={data} />
        <VideoStreamSetLogoFromEPGDialog value={data} />
        <VideoStreamVisibleDialog iconFilled={false} id={dataKey} skipOverLayer values={[data]} />
        <VideoStreamSetAutoSetEPGDialog iconFilled={false} id={dataKey} skipOverLayer values={[data]} />
        <VideoStreamDeleteDialog iconFilled={false} id={dataKey} values={[data]} />
        <VideoStreamEditDialog value={data} />
      </div>
    ),
    [dataKey]
  );

  const columns = useMemo((): ColumnMeta[] => {
    const columnConfigs = [
      channelNumberColumnConfig,
      channelLogoColumnConfig,
      channelNameColumnConfig,
      channelGroupConfig,
      epgColumnConfig,
      m3uFileNameColumnConfig
    ];

    columnConfigs.push({
      bodyTemplate: targetActionBodyTemplate,
      field: 'isHidden',
      header: 'Actions',
      isHidden: !enableEdit,
      resizeable: false,
      sortable: false,
      width: '9rem'
    });

    return columnConfigs;
  }, [
    channelGroupConfig,
    channelLogoColumnConfig,
    channelNameColumnConfig,
    channelNumberColumnConfig,
    enableEdit,
    epgColumnConfig,
    m3uFileNameColumnConfig,
    targetActionBodyTemplate
  ]);

  const rightHeaderTemplate = useMemo(
    () => (
      <div className="flex justify-content-end align-items-center w-full gap-1">
        <TriSelectShowHidden dataKey={dataKey} />
        <VideoStreamSetTimeShiftsDialog id={dataKey} />
        <VideoStreamResetLogosDialog id={dataKey} />
        <VideoStreamSetLogosFromEPGDialog id={dataKey} />
        <AutoSetChannelNumbers id={dataKey} />
        <VideoStreamVisibleDialog id={dataKey} />
        <VideoStreamSetAutoSetEPGDialog iconFilled id={dataKey} />
        <VideoStreamDeleteDialog iconFilled id={dataKey} />
        <VideoStreamAddDialog group={channelGroupNames?.[0]} />
      </div>
    ),
    [dataKey, channelGroupNames]
  );

  const leftHeaderTemplate = useMemo(
    () => (
      <div className="flex justify-content-end align-items-center w-full gap-1">
        <PlayListDataSelectorDropDown id={id} />
      </div>
    ),
    [id]
  );

  return (
    <DataSelector
      columns={columns}
      defaultSortField="user_Tvg_name"
      defaultSortOrder={1}
      emptyMessage="No Streams"
      headerName={GetMessage('streams')}
      headerLeftTemplate={leftHeaderTemplate}
      headerRightTemplate={rightHeaderTemplate}
      id={dataKey}
      onSelectionChange={(value, selectAll) => {
        if (selectAll !== true) {
          setSelectedVideoStreams(value as VideoStreamDto[]);
        }
      }}
      queryFilter={useVideoStreamsGetPagedVideoStreamsQuery}
      reorderable={reorderable}
      selectedItemsKey="selectSelectedVideoStreamDtoItems"
      selectionMode="multiple"
      style={{ height: 'calc(100vh - 40px)' }}
    />
  );
};

export default memo(ChannelGroupVideoStreamDataSelector);
