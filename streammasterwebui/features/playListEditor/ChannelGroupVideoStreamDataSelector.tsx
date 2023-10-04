import { arraysContainSameStrings, GetMessage } from '@/lib/common/common'
import {
  ChannelGroupDto,
  useVideoStreamsGetPagedVideoStreamsQuery,
  VideoStreamDto,
} from '@/lib/iptvApi'
import { useQueryAdditionalFilters } from '@/lib/redux/slices/useQueryAdditionalFilters'
import { useSelectedItems } from '@/lib/redux/slices/useSelectedItemsSlice'
import { useSelectedVideoStreams } from '@/lib/redux/slices/useSelectedVideoStreams'
import {
  useChannelGroupColumnConfig,
  useChannelLogoColumnConfig,
  useChannelNameColumnConfig,
  useChannelNumberColumnConfig,
  useEPGColumnConfig,
} from '@/src/components/columns/columnConfigHooks'
import DataSelector from '@/src/components/dataSelector/DataSelector'
import { ColumnMeta } from '@/src/components/dataSelector/DataSelectorTypes'
import { TriSelect } from '@/src/components/selectors/TriSelect'
import AutoSetChannelNumbers from '@/src/components/videoStream/AutoSetChannelNumbers'
import VideoStreamAddDialog from '@/src/components/videoStream/VideoStreamAddDialog'
import VideoStreamDeleteDialog from '@/src/components/videoStream/VideoStreamDeleteDialog'
import VideoStreamEditDialog from '@/src/components/videoStream/VideoStreamEditDialog'
import VideoStreamResetLogoDialog from '@/src/components/videoStream/VideoStreamResetLogoDialog'
import VideoStreamResetLogosDialog from '@/src/components/videoStream/VideoStreamResetLogosDialog'
import VideoStreamSetAutoSetEPGDialog from '@/src/components/videoStream/VideoStreamSetAutoSetEPGDialog'
import VideoStreamSetLogoFromEPGDialog from '@/src/components/videoStream/VideoStreamSetLogoFromEPGDialog'
import VideoStreamSetLogosFromEPGDialog from '@/src/components/videoStream/VideoStreamSetLogosFromEPGDialog'
import VideoStreamVisibleDialog from '@/src/components/videoStream/VideoStreamVisibleDialog'
import {
  memo,
  useCallback,
  useEffect,
  useMemo,
  useState,
  type CSSProperties,
} from 'react'

type ChannelGroupVideoStreamDataSelectorProps = {
  readonly enableEdit?: boolean
  readonly id: string
  readonly reorderable?: boolean
}

const ChannelGroupVideoStreamDataSelector = ({
  enableEdit: propsEnableEdit,
  id,
  reorderable,
}: ChannelGroupVideoStreamDataSelectorProps) => {
  const dataKey = id + '-ChannelGroupVideoStreamDataSelector'
  const { selectSelectedItems } = useSelectedItems<ChannelGroupDto>(
    'selectSelectedChannelGroupDtoItems',
  )
  const [enableEdit, setEnableEdit] = useState<boolean>(true)
  const { columnConfig: epgColumnConfig } = useEPGColumnConfig({
    enableEdit: enableEdit,
  })
  const { columnConfig: channelNumberColumnConfig } =
    useChannelNumberColumnConfig({ enableEdit: enableEdit, useFilter: false })
  const { columnConfig: channelNameColumnConfig } = useChannelNameColumnConfig({
    enableEdit: enableEdit,
  })
  const { columnConfig: channelLogoColumnConfig } = useChannelLogoColumnConfig({
    enableEdit: enableEdit,
  })

  const channelGroupNames = useMemo(() => {
    return selectSelectedItems.map((channelGroup) => channelGroup.name)
  }, [selectSelectedItems])

  const { columnConfig: channelGroupConfig } = useChannelGroupColumnConfig({
    enableEdit: enableEdit,
    values: [...(channelGroupNames ?? [])].sort(),
  })
  const { queryAdditionalFilter, setQueryAdditionalFilter } =
    useQueryAdditionalFilters(dataKey)
  const { setSelectedVideoStreams } = useSelectedVideoStreams(dataKey)

  useEffect(() => {
    if (
      !arraysContainSameStrings(
        queryAdditionalFilter?.values,
        channelGroupNames,
      )
    ) {
      setQueryAdditionalFilter({
        field: 'user_Tvg_group',
        matchMode: 'equals',
        values: channelGroupNames,
      })
    }
  }, [
    channelGroupNames,
    dataKey,
    queryAdditionalFilter,
    setQueryAdditionalFilter,
  ])

  useEffect(() => {
    if (propsEnableEdit != enableEdit) {
      setEnableEdit(propsEnableEdit ?? true)
    }

    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [propsEnableEdit])

  const targetActionBodyTemplate = useCallback(
    (data: VideoStreamDto) => {
      return (
        <div className="flex p-0 justify-content-end align-items-center">
          <VideoStreamResetLogoDialog value={data} />
          <VideoStreamSetLogoFromEPGDialog value={data} />
          <VideoStreamVisibleDialog
            iconFilled={false}
            id={dataKey}
            skipOverLayer
            values={[data]}
          />
          <VideoStreamSetAutoSetEPGDialog
            iconFilled={false}
            id={dataKey}
            skipOverLayer
            values={[data]}
          />
          <VideoStreamDeleteDialog
            iconFilled={false}
            id={dataKey}
            values={[data]}
          />
          <VideoStreamEditDialog value={data} />
        </div>
      )
    },
    [dataKey],
  )

  const targetColumns = useMemo((): ColumnMeta[] => {
    let columnConfigs = [
      channelNumberColumnConfig,
      channelLogoColumnConfig,
      channelNameColumnConfig,
      channelGroupConfig,
      epgColumnConfig,
    ]

    // columnConfigs.push(channelGroupConfig);
    // columnConfigs.push(epgColumnConfig);

    columnConfigs.push({
      bodyTemplate: targetActionBodyTemplate,
      field: 'isHidden',
      header: 'Actions',
      isHidden: !enableEdit,
      resizeable: false,
      sortable: false,
      style: {
        maxWidth: '7rem',
        width: '7rem',
      } as CSSProperties,
    })

    return columnConfigs
  }, [
    channelGroupConfig,
    channelLogoColumnConfig,
    channelNameColumnConfig,
    channelNumberColumnConfig,
    enableEdit,
    epgColumnConfig,
    targetActionBodyTemplate,
  ])

  const rightHeaderTemplate = useMemo(() => {
    return (
      <div className="flex justify-content-end align-items-center w-full gap-1">
        <TriSelect dataKey={dataKey} />
        <VideoStreamResetLogosDialog id={dataKey} />
        <VideoStreamSetLogosFromEPGDialog id={dataKey} />
        <AutoSetChannelNumbers id={dataKey} />
        <VideoStreamVisibleDialog id={dataKey} />
        <VideoStreamSetAutoSetEPGDialog iconFilled id={dataKey} />
        <VideoStreamDeleteDialog iconFilled id={dataKey} />
        <VideoStreamAddDialog group={channelGroupNames?.[0]} />
      </div>
    )
  }, [dataKey, channelGroupNames])

  return (
    <DataSelector
      columns={targetColumns}
      defaultSortField="user_tvg_name"
      defaultSortOrder={1}
      emptyMessage="No Streams"
      headerName={GetMessage('streams')}
      headerRightTemplate={rightHeaderTemplate}
      id={dataKey}
      onSelectionChange={(value, selectAll) => {
        if (selectAll !== true) {
          setSelectedVideoStreams(value as VideoStreamDto[])
        }
      }}
      queryFilter={useVideoStreamsGetPagedVideoStreamsQuery}
      reorderable={reorderable}
      selectedItemsKey="selectSelectedVideoStreamDtoItems"
      selectionMode="multiple"
      style={{ height: 'calc(100vh - 40px)' }}
    />
  )
}

export default memo(ChannelGroupVideoStreamDataSelector)
